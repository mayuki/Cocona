using Cocona.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Cocona.Command
{
    public class CoconaCommandProvider : ICoconaCommandProvider
    {
        private readonly Type[] _targetTypes;
        private static readonly Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>> _emptyOverloads = new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>();
        private readonly Lazy<CommandCollection> _commandCollection;
        private readonly bool _treatPublicMethodsAsCommands;
        private readonly bool _enableConvertOptionNameToLowerCase;
        private readonly bool _enableConvertCommandNameToLowerCase;

        public CoconaCommandProvider(Type[] targetTypes, bool treatPublicMethodsAsCommands = true, bool enableConvertOptionNameToLowerCase = false, bool enableConvertCommandNameToLowerCase = false)
        {
            _targetTypes = targetTypes ?? throw new ArgumentNullException(nameof(targetTypes));
            _commandCollection = new Lazy<CommandCollection>(GetCommandCollectionCore, LazyThreadSafetyMode.None);
            _treatPublicMethodsAsCommands = treatPublicMethodsAsCommands;
            _enableConvertOptionNameToLowerCase = enableConvertOptionNameToLowerCase;
            _enableConvertCommandNameToLowerCase = enableConvertCommandNameToLowerCase;
        }

        public CommandCollection GetCommandCollection()
            => _commandCollection.Value;

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private CommandCollection GetCommandCollectionCore()
        {
            var commandMethods = new List<MethodInfo>(10);
            var overloadCommandMethods = new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(10);

            // Command types
            foreach (var type in _targetTypes)
            {
                if (type.IsAbstract || (type.IsGenericType && type.IsConstructedGenericType)) continue;

                if (type.GetCustomAttribute<IgnoreAttribute>() != null) continue;

                // Command methods
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (method.IsSpecialName || method.DeclaringType == typeof(object)) continue;

                    if (
                        (_treatPublicMethodsAsCommands && method.IsPublic) ||
                        (method.GetCustomAttribute<CommandAttribute>(inherit: true) != null) ||
                        (method.GetCustomAttribute<PrimaryCommandAttribute>(inherit: true) != null)
                    )
                    {
                        if (method.GetCustomAttribute<IgnoreAttribute>() != null) continue;

                        var commandOverloadAttr = method.GetCustomAttribute<CommandOverloadAttribute>();
                        if (commandOverloadAttr != null)
                        {
                            if (!overloadCommandMethods.TryGetValue(commandOverloadAttr.TargetCommand, out var overloads))
                            {
                                overloads = new List<(MethodInfo Method, CommandOverloadAttribute Attribute)>();
                                overloadCommandMethods.Add(commandOverloadAttr.TargetCommand, overloads);
                            }
                            overloads.Add((method, commandOverloadAttr));
                        }
                        else
                        {
                            commandMethods.Add(method);
                        }
                    }
                }
            }

            var hasMultipleCommand = commandMethods.Count > 1;
            var commandNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var commands = new List<CommandDescriptor>(commandMethods.Count);
            foreach (var commandMethod in commandMethods)
            {
                var command = CreateCommand(commandMethod, !hasMultipleCommand, overloadCommandMethods);
                if (commandNames.Contains(command.Name))
                {
                    throw new CoconaException($"Command '{command.Name}' has already exists. (Method: {command.Method.Name})");
                }
                commandNames.Add(command.Name);

                if (command.Aliases.Count != 0)
                {
                    foreach (var alias in command.Aliases)
                    {
                        if (commandNames.Contains(alias))
                        {
                            throw new CoconaException($"Command alias '{alias}' has already exists in commands. (Method: {command.Method.Name})");
                        }
                        commandNames.Add(alias);
                    }
                }

                commands.Add(command);
            }

            return new CommandCollection(commands);
        }

        // NOTE: Avoid JIT optimization to improve responsiveness at first time.
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public CommandDescriptor CreateCommand(MethodInfo methodInfo, bool isSingleCommand, Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>> overloadCommandMethods)
        {
            ThrowHelper.ArgumentNull(methodInfo, nameof(methodInfo));

            var commandAttr = methodInfo.GetCustomAttribute<CommandAttribute>();
            var commandName = commandAttr?.Name ?? methodInfo.Name;
            var description = commandAttr?.Description ?? string.Empty;
            var aliases = commandAttr?.Aliases ?? Array.Empty<string>();

            if (_enableConvertCommandNameToLowerCase) commandName = ToCommandCase(commandName);

            var isPrimaryCommand = methodInfo.GetCustomAttribute<PrimaryCommandAttribute>() != null;
            var isHidden = methodInfo.GetCustomAttribute<HiddenAttribute>() != null;

            var allOptions = new Dictionary<string, CommandOptionDescriptor>(StringComparer.OrdinalIgnoreCase);
            var allOptionShortNames = new HashSet<char>();

            var methodParameters = methodInfo.GetParameters();

            var parameters = new List<ICommandParameterDescriptor>(methodParameters.Length);
            var options = new List<CommandOptionDescriptor>(methodParameters.Length);
            var arguments = new List<CommandArgumentDescriptor>(methodParameters.Length);

            var defaultArgOrder = 0;

            for (var i = 0; i < methodParameters.Length; i++)
            {
                var methodParam = methodParameters[i];
                var defaultValue = methodParam.HasDefaultValue ? new CoconaDefaultValue(methodParam.DefaultValue) : CoconaDefaultValue.None;

                var ignoreAttr = methodParam.GetCustomAttribute<IgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    var ignoreParamDescriptor = new CommandIgnoredParameterDescriptor(
                        methodParam.ParameterType,
                        methodParam.Name,
                        methodParam.HasDefaultValue
                            ? methodParam.DefaultValue
                            : methodParam.ParameterType.IsValueType
                                ? Activator.CreateInstance(methodParam.ParameterType)
                                : null
                    );

                    parameters.Add(ignoreParamDescriptor);
                    continue;
                }

                var argumentAttr = methodParam.GetCustomAttribute<ArgumentAttribute>();
                if (argumentAttr != null)
                {
                    if (!isSingleCommand && isPrimaryCommand) throw new CoconaException("A primary command with multiple commands cannot handle/have any arguments.");

                    var argName = argumentAttr.Name ?? methodParam.Name;
                    var argDesc = argumentAttr.Description ?? string.Empty;
                    var argOrder = argumentAttr.Order != 0 ? argumentAttr.Order : defaultArgOrder;

                    defaultArgOrder++;

                    var commandArgDescriptor = new CommandArgumentDescriptor(
                        methodParam.ParameterType,
                        argName,
                        argOrder,
                        argDesc,
                        defaultValue,
                        methodParam.GetCustomAttributes(true).OfType<Attribute>().ToArray());

                    parameters.Add(commandArgDescriptor);
                    arguments.Add(commandArgDescriptor);
                    continue;
                }

                var fromServiceAttr = methodParam.GetCustomAttribute<FromServiceAttribute>();
                if (fromServiceAttr != null)
                {
                    var serviceParamDescriptor = new CommandServiceParameterDescriptor(methodParam.ParameterType, methodParam.Name);
                    parameters.Add(serviceParamDescriptor);
                    continue;
                }

                var optionAttr = methodParam.GetCustomAttribute<OptionAttribute>();
                {
                    var optionName = optionAttr?.Name ?? methodParam.Name;
                    var optionDesc = optionAttr?.Description ?? string.Empty;
                    var optionShortNames = optionAttr?.ShortNames ?? Array.Empty<char>();
                    var optionValueName = optionAttr?.ValueName ?? (DynamicListHelper.IsArrayOrEnumerableLike(methodParam.ParameterType) ? DynamicListHelper.GetElementType(methodParam.ParameterType) : methodParam.ParameterType).Name;
                    var optionIsHidden = methodParam.GetCustomAttribute<HiddenAttribute>() != null;

                    if (_enableConvertOptionNameToLowerCase) optionName = ToCommandCase(optionName);

                    // If the option type is bool, the option has always default value (false).
                    if (!defaultValue.HasValue && methodParam.ParameterType == typeof(bool))
                    {
                        defaultValue = new CoconaDefaultValue(false);
                    }

                    if (allOptions.ContainsKey(optionName))
                        throw new CoconaException($"Option '{optionName}' is already exists.");
                    if (allOptionShortNames.Count != 0 && optionShortNames.Count != 0 && allOptionShortNames.IsSupersetOf(optionShortNames))
                        throw new CoconaException($"Short name option '{string.Join(",", optionShortNames)}' is already exists.");

                    var optionDescriptor = new CommandOptionDescriptor(
                        methodParam.ParameterType,
                        optionName,
                        optionShortNames,
                        optionDesc,
                        defaultValue,
                        optionValueName,
                        optionIsHidden ? CommandOptionFlags.Hidden : CommandOptionFlags.None,
                        methodParam.GetCustomAttributes<Attribute>(true).ToArray());
                    allOptions.Add(optionName, optionDescriptor);
                    allOptionShortNames.UnionWith(optionShortNames);

                    options.Add(optionDescriptor);
                    parameters.Add(optionDescriptor);
                }
            }

            // Overloaded commands
            var overloadDescriptors = new List<CommandOverloadDescriptor>();
            if (overloadCommandMethods.TryGetValue(commandName, out var overloads))
            {
                overloadDescriptors.AddRange(overloads
                    .Select(x => new CommandOverloadDescriptor(
                        (allOptions.TryGetValue(x.Attribute.OptionName, out var name) ? name : throw new CoconaException($"Command option overload '{x.Attribute.OptionName}' was not found in overload target '{methodInfo.Name}'.")),
                        x.Attribute.OptionValue,
                        CreateCommand(x.Method, isSingleCommand, _emptyOverloads),
                        x.Attribute.Comparer != null ? (IEqualityComparer<string>)Activator.CreateInstance(x.Attribute.Comparer) : null
                    )));
            }

            var flags = ((isHidden) ? CommandFlags.Hidden : CommandFlags.None) |
                        ((isSingleCommand || isPrimaryCommand) ? CommandFlags.Primary : CommandFlags.None);

            return new CommandDescriptor(
                methodInfo,
                commandName,
                aliases,
                description,
                parameters,
                options,
                arguments,
                overloadDescriptors.ToArray(),
                flags
            );
        }

        public static string ToCommandCase(string value)
        {
            var sb = new StringBuilder(value.Length);
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (Char.IsUpper(c))
                {
                    if (sb.Length != 0 && Char.IsLower(value[i - 1]))
                    {
                        sb.Append('-');
                    }
                    sb.Append(Char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
