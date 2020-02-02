using Cocona.CommandLine;
using Cocona.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocona.Command.Binder.Validation;

namespace Cocona.Command.Binder
{
    public class CoconaParameterBinder : ICoconaParameterBinder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICoconaValueConverter _valueConverter;
        private readonly ICoconaParameterValidatorProvider _validatorProvider;

        public CoconaParameterBinder(IServiceProvider serviceProvider, ICoconaValueConverter valueConverter, ICoconaParameterValidatorProvider validatorProvider)
        {
            _serviceProvider = serviceProvider;
            _valueConverter = valueConverter;
            _validatorProvider = validatorProvider;
        }

        public object?[] Bind(CommandDescriptor commandDescriptor, IReadOnlyList<CommandOption> commandOptionValues, IReadOnlyList<CommandArgument> commandArgumentValues)
        {
            var bindParams = new object?[commandDescriptor.Parameters.Count];
            var optionValueByOption = commandOptionValues.ToLookup(k => k.Option, v => v.Value);

            var orderedArgDescWithParamIndex = new List<(CommandArgumentDescriptor Argument, int ParameterIndex)>(commandDescriptor.Parameters.Count);
            var index = 0;
            foreach (var param in commandDescriptor.Parameters)
            {
                switch (param)
                {
                    case CommandOptionDescriptor optionDesc:
                        if (optionValueByOption.Contains(optionDesc))
                        {
                            bindParams[index++] = CreateValue(optionDesc.OptionType, optionValueByOption[optionDesc].ToArray(), optionDesc, null);
                        }
                        else if (!optionDesc.IsRequired)
                        {
                            bindParams[index++] = optionDesc.DefaultValue.Value;
                        }
                        else
                        {
                            throw new ParameterBinderException(ParameterBinderResult.InsufficientOption, optionDesc);
                        }
                        break;
                    case CommandArgumentDescriptor argumentDesc:
                        // NOTE: Skip processing arguments at here.
                        orderedArgDescWithParamIndex.Add((argumentDesc, index++));
                        break;
                    case CommandServiceParameterDescriptor serviceParamDesc:
                        bindParams[index++] = _serviceProvider.GetService(serviceParamDesc.ServiceType);
                        break;
                    case CommandIgnoredParameterDescriptor ignoredDesc:
                        bindParams[index++] = ignoredDesc.DefaultValue;
                        break;
                    default:
                        throw new ParameterBinderException(ParameterBinderResult.TypeNotSupported, $"CoconaParameterBinder doesn't support '{param.GetType().FullName}' as bind target. Param: {param}");
                }
            }

            // arguments
            orderedArgDescWithParamIndex.Sort((a, b) => a.Argument.Order.CompareTo(b.Argument.Order));
            index = 0;

            for (var i = 0; i < orderedArgDescWithParamIndex.Count; i++)
            {
                var argDesc = orderedArgDescWithParamIndex[i];
                if (commandArgumentValues.Count == index)
                {
                    if (!argDesc.Argument.IsRequired)
                    {
                        bindParams[argDesc.ParameterIndex] = argDesc.Argument.DefaultValue.Value;
                        continue;
                    }

                    throw new ParameterBinderException(ParameterBinderResult.InsufficientArgument, argument: argDesc.Argument);
                }

                // T[] or List<T>, IEnumerable<T> ...
                if (argDesc.Argument.IsEnumerableLike)
                {
                    // pick values from tail of arguments.
                    // e.g: [ string, string[],   string, string ]
                    //          |     ^^^^^^^^      |       |
                    //      [ arg0,   arg1, arg2,  arg3,   arg4 ]
                    //                              <-------o
                    var indexRev = commandArgumentValues.Count - 1;
                    for (var j = orderedArgDescWithParamIndex.Count - 1; j > i; j--)
                    {
                        var argDesc2 = orderedArgDescWithParamIndex[j];

                        if (indexRev == index) throw new ParameterBinderException(ParameterBinderResult.InsufficientArgument, argument: argDesc2.Argument);
                        if (argDesc2.Argument.IsEnumerableLike) throw new ParameterBinderException(ParameterBinderResult.MultipleArrayInArgument, argument: argDesc2.Argument);

                        bindParams[argDesc2.ParameterIndex] = ConvertTo(argDesc2.Argument, argDesc2.Argument.ArgumentType, commandArgumentValues[indexRev--].Value);
                    }

                    // pick rest values to array argment.
                    // e.g: [ string,  string[],  string, string ]
                    //          |       |    |      |       |
                    //      [ arg0,  [arg1, arg2], arg3,   arg4 ]
                    var rest = commandArgumentValues.Skip(index).Take((indexRev + 1) - index);
                    bindParams[argDesc.ParameterIndex] = CreateValue(argDesc.Argument.ArgumentType, rest.Select(x => x.Value).ToArray(), null, argDesc.Argument);

                    return bindParams;
                }

                bindParams[argDesc.ParameterIndex] = ConvertTo(argDesc.Argument, argDesc.Argument.ArgumentType, commandArgumentValues[index++].Value);
            }

            return bindParams;
        }

        private object? Validate(ICommandParameterDescriptor commandParameter, object? value)
        {
            var ctx = new CoconaParameterValidationContext(commandParameter, value);
            foreach (var validator in _validatorProvider.CreateValidators(commandParameter))
            {
                var validationFailed = validator.Validate(ctx).FirstOrDefault();
                if (validationFailed != null)
                {
                    throw new ParameterBinderException(ParameterBinderResult.ValidationFailed, validationFailed.Message);
                }
            }

            return value;
        }

        private object? ConvertTo(CommandOptionDescriptor option, Type type, string value)
        {
            try
            {
                return Validate(option, _valueConverter.ConvertTo(type, value));
            }
            catch (Exception ex) when (!(ex is ParameterBinderException))
            {
                throw new ParameterBinderException(ParameterBinderResult.TypeNotSupported, $"Option '{option.Name}' requires {type.Name} value. '{value}' cannot be converted to {type.Name} value.", option: option, innerException: ex);
            }
        }

        private object? ConvertTo(CommandArgumentDescriptor argument, Type type, string value)
        {
            try
            {
                return Validate(argument, _valueConverter.ConvertTo(type, value));
            }
            catch (Exception ex) when (!(ex is ParameterBinderException))
            {
                throw new ParameterBinderException(ParameterBinderResult.TypeNotSupported, $"Argument '{argument.Name}' requires {type.Name} value. '{value}' cannot be converted to {type.Name} value.", argument: argument, innerException: ex);
            }
        }

        private object? CreateValue(Type valueType, string?[] values, CommandOptionDescriptor? option, CommandArgumentDescriptor? argument)
        {
            if (DynamicListHelper.TryCreateArrayOrEnumerableLike(valueType, values, _valueConverter, out var arrayOrEnumerableLike))
            {
                // T[] or List<T> (IEnumerable<T>, IList<T>)
                return arrayOrEnumerableLike;
            }
            else if (!DynamicListHelper.IsArrayOrEnumerableLike(valueType))
            {
                // Primitive or plain object (int, bool, string ...)
                var value = values[values.Length - 1];
                if (value == null) throw new ParameterBinderException(ParameterBinderResult.InsufficientOptionValue, option, argument);

                return (option != null) ? ConvertTo(option!, valueType, value) : ConvertTo(argument!, valueType, value);
            }

            throw new ParameterBinderException(ParameterBinderResult.TypeNotSupported, $"Cannot create a instance of type '{valueType.FullName}'", option, argument);
        }
    }
}
