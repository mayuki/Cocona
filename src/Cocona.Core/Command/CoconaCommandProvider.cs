using Cocona.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Cocona.Command
{
    public class CoconaCommandProvider : ICoconaCommandProvider
    {
        private readonly Type[] _targetTypes;
        private readonly MethodInfo[] _targetMethods;
        
        private static readonly Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>> _emptyOverloads = new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>();
        private readonly bool _treatPublicMethodsAsCommands;
        private readonly bool _enableConvertOptionNameToLowerCase;
        private readonly bool _enableConvertCommandNameToLowerCase;
        private readonly bool _enableConvertArgumentNameToLowerCase;

        public CoconaCommandProvider(Type[] targetTypes, MethodInfo[]? targetMethods = default, bool treatPublicMethodsAsCommands = true, bool enableConvertOptionNameToLowerCase = false, bool enableConvertCommandNameToLowerCase = false, bool enableConvertArgumentNameToLowerCase = false)
        {
            _targetTypes = targetTypes ?? throw new ArgumentNullException(nameof(targetTypes));
            _targetMethods = targetMethods ?? Array.Empty<MethodInfo>();
            _treatPublicMethodsAsCommands = treatPublicMethodsAsCommands;
            _enableConvertOptionNameToLowerCase = enableConvertOptionNameToLowerCase;
            _enableConvertCommandNameToLowerCase = enableConvertCommandNameToLowerCase;
            _enableConvertArgumentNameToLowerCase = enableConvertArgumentNameToLowerCase;
        }

        public CommandCollection GetCommandCollection()
            => GetCommandCollectionCore(_targetTypes, _targetMethods);

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private CommandCollection GetCommandCollectionCore(IReadOnlyList<Type> targetTypes, IReadOnlyList<MethodInfo> targetMethods)
        {
            var commandMethods = new List<MethodInfo>(10);
            var overloadCommandMethods = new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>(10);
            var subCommandEntryPoints = new List<CommandDescriptor>();

            // Command types
            foreach (var type in targetTypes)
            {
                if (type.IsAbstract || (type.IsGenericType && type.IsConstructedGenericType)) continue;

                if (type.GetCustomAttribute<IgnoreAttribute>() != null) continue;

                // Command methods
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (method.IsSpecialName || method.DeclaringType == typeof(object)) continue;
                    if (!_treatPublicMethodsAsCommands && !method.IsPublic) continue;

                    var implicitCommand = (_treatPublicMethodsAsCommands && method.IsPublic);
                    AddCommandMethod(method, implicitCommand);
                }

                // Nested sub-commands
                var subCommandsAttrs = type.GetCustomAttributes<HasSubCommandsAttribute>();
                foreach (var subCommandsAttr in subCommandsAttrs)
                {
                    if (subCommandsAttr.Type == type) throw new InvalidOperationException("Sub-commands type must not be same as command type.");

                    var subCommands = GetCommandCollectionCore(new[] { subCommandsAttr.Type }, Array.Empty<MethodInfo>());
                    var commandName = subCommandsAttr.Type.Name;
                    if (!string.IsNullOrWhiteSpace(subCommandsAttr.CommandName))
                    {
                        commandName = subCommandsAttr.CommandName!;
                    }

                    if (_enableConvertCommandNameToLowerCase) commandName = ToCommandCase(commandName);

                    var dummyMethod = ((Action)(() => { })).Method;
                    var command = new CommandDescriptor(
                        dummyMethod,
                        commandName,
                        Array.Empty<string>(),
                        subCommandsAttr.Description ?? subCommands.Description,
                        Array.Empty<ICommandParameterDescriptor>(),
                        Array.Empty<CommandOptionDescriptor>(),
                        Array.Empty<CommandArgumentDescriptor>(),
                        Array.Empty<CommandOverloadDescriptor>(),
                        Array.Empty<CommandOptionLikeCommandDescriptor>(),
                        CommandFlags.SubCommandsEntryPoint,
                        subCommands
                    );
                    subCommandEntryPoints.Add(command);
                }
            }
            
            // Command methods
            foreach (var method in targetMethods)
            {
                AddCommandMethod(method, implicitCommand: true);
            }

            var hasMultipleCommand = commandMethods.Count > 1 || subCommandEntryPoints.Count != 0;
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

            commands.AddRange(subCommandEntryPoints);

            return new CommandCollection(commands);

            void AddCommandMethod(MethodInfo method, bool implicitCommand)
            {
                var (commandAttr, primaryCommandAttr, ignoreAttribute, commandOverloadAttr)
                    = AttributeHelper
                        .GetAttributes<CommandAttribute, PrimaryCommandAttribute, IgnoreAttribute, CommandOverloadAttribute>(
                            method.GetCustomAttributes(typeof(Attribute), true));

                if (implicitCommand || commandAttr != null || primaryCommandAttr != null)
                {
                    if (ignoreAttribute != null) return;

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
        
        // NOTE: Avoid JIT optimization to improve responsiveness at first time.
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public CommandDescriptor CreateCommand(MethodInfo methodInfo, bool isSingleCommand, Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>> overloadCommandMethods)
        {
            ThrowHelper.ArgumentNull(methodInfo, nameof(methodInfo));
            
            // Collect Method attributes
            var commandMethodDesc = GetCommandMethodDescriptor(methodInfo);
            var commandAttr = commandMethodDesc.CommandAttribute;
            var commandName = commandAttr?.Name ?? methodInfo.Name;
            var description = commandAttr?.Description ?? string.Empty;
            var aliases = commandAttr?.Aliases ?? Array.Empty<string>();

            var isPrimaryCommand = commandMethodDesc.IsPrimaryCommand;
            var isHidden = commandMethodDesc.IsHidden;
            var isIgnoreUnknownOptions = commandMethodDesc.IsIgnoreUnknownOptions;

            // If the command is not only one, the command name must be specified. 
            if (!isSingleCommand)
            {
                if (string.IsNullOrEmpty(commandName) || Regex.IsMatch(commandName, "[<>|]"))
                {
                    throw new CoconaException($"The command name contains invalid character. (Name: {commandName}, Method: {methodInfo.Name})");
                }
            }

            // If the command method should forward to another command.
            if (commandMethodDesc.CommandMethodForwardedTo is { } cmdForwardedTo)
            {
                var forwardTargetMethodInfo = cmdForwardedTo.CommandType.GetMethod(cmdForwardedTo.CommandMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                methodInfo = forwardTargetMethodInfo
                             ?? throw new InvalidOperationException($"The command '{methodInfo.Name}' is specified for command method forwarding. But the destination command '{cmdForwardedTo.CommandType.Name}.{cmdForwardedTo.CommandMethodName} was not found.");
            }

            var methodParameters = methodInfo.GetParameters();
            var builder = new CommandDescriptorBuilder(_enableConvertOptionNameToLowerCase, _enableConvertArgumentNameToLowerCase);

            CollectParameters(methodInfo, methodParameters, builder, isPrimaryCommand, isSingleCommand);

            // Overloaded commands
            var overloadDescriptors = Array.Empty<CommandOverloadDescriptor>();
            if (overloadCommandMethods.TryGetValue(commandName, out var overloads))
            {
                overloadDescriptors = new CommandOverloadDescriptor[overloads.Count];
                for (var i = 0; i < overloadDescriptors.Length; i++)
                {
                    var overload = overloads[i];
                    var overloadDescriptor = new CommandOverloadDescriptor(
                        (builder.AllOptions.TryGetValue(overload.Attribute.OptionName, out var name) ? name : throw new CoconaException($"Command option overload '{overload.Attribute.OptionName}' was not found in overload target '{methodInfo.Name}'.")),
                        overload.Attribute.OptionValue,
                        CreateCommand(overload.Method, isSingleCommand, _emptyOverloads),
                        overload.Attribute.Comparer != null ? (IEqualityComparer<string>?)Activator.CreateInstance(overload.Attribute.Comparer) : null
                    );

                    overloadDescriptors[i] = overloadDescriptor;
                }
            }

            // OptionLikeCommands
            var optionLikeCommands = Array.Empty<CommandOptionLikeCommandDescriptor>();
            if (commandMethodDesc.OptionLikeCommands.Any())
            {
                optionLikeCommands = commandMethodDesc.OptionLikeCommands
                    .Select(x =>
                    {
                        var methodInfoOptionLikeCommandTarget = x.CommandType.GetMethod(x.CommandMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (methodInfoOptionLikeCommandTarget is null)
                        {
                            throw new InvalidOperationException($"A method of option-like command '{x.CommandMethodName}' was not found in '{x.CommandType}'");
                        }

                        var optionLikeCommandDesc = CreateCommand(methodInfoOptionLikeCommandTarget, false, _emptyOverloads);
                        return new CommandOptionLikeCommandDescriptor(
                            x.OptionName,
                            x.ShortNames,
                            optionLikeCommandDesc,
                            CommandOptionFlags.OptionLikeCommand | (optionLikeCommandDesc.IsHidden ? CommandOptionFlags.Hidden : CommandOptionFlags.None)
                        );
                    })
                    .ToArray();
            }

            // Convert the command name to lower-case
            if (_enableConvertCommandNameToLowerCase)
            {
                commandName = ToCommandCase(commandName);
            }

            var flags = (isHidden ? CommandFlags.Hidden : CommandFlags.None) |
                        ((isSingleCommand || isPrimaryCommand) ? CommandFlags.Primary : CommandFlags.None) |
                        (isIgnoreUnknownOptions ? CommandFlags.IgnoreUnknownOptions : CommandFlags.None);

            var (parameters, options, arguments) = builder.Build();
            return new CommandDescriptor(
                methodInfo,
                commandName,
                aliases,
                description,
                parameters,
                options,
                arguments,
                overloadDescriptors,
                optionLikeCommands,
                flags,
                null
            );
        }

        private static void CollectParameters(MemberInfo memberInfo, ParameterInfo[] methodParameters, CommandDescriptorBuilder builder, bool isPrimaryCommand, bool isSingleCommand)
        {
            for (var i = 0; i < methodParameters.Length; i++)
            {
                var methodParam = methodParameters[i];
                var defaultValue = methodParam.HasDefaultValue ? new CoconaDefaultValue(methodParam.DefaultValue) : CoconaDefaultValue.None;

                // Collect Parameter attributes
                var attrs = new CommandParameterAttributeSet(methodParam.GetCustomAttributes(typeof(Attribute), true));

                if (methodParam.Name is null)
                {
                    throw new CoconaException($"An unnamed parameter is not supported. (Method: {memberInfo.Name})");
                }

                if (attrs.Ignore != null)
                {
                    builder.AddIgnore(
                        methodParam.ParameterType,
                        methodParam.Name,
                        methodParam.HasDefaultValue
                            ? methodParam.DefaultValue
                            : methodParam.ParameterType.IsValueType
                                ? Activator.CreateInstance(methodParam.ParameterType)
                                : null
                    );
                    continue;
                }

                // If a parameter has no OptionAttribute and a type of the parameter implements ICommandParameterSet
                if (typeof(ICommandParameterSet).IsAssignableFrom(methodParam.ParameterType))
                {
                    var paramSetType = methodParam.ParameterType;
                    if (paramSetType.IsAbstract || paramSetType.IsInterface)
                    {
                        throw new CoconaException($"The parameter set '{paramSetType.FullName}' must be non-abstract class.");
                    }

                    var ctors = paramSetType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                    if (!ctors.Any())
                    {
                        throw new CoconaException($"The parameter set '{paramSetType.FullName}' has no public constructor.");
                    }

                    if (ctors.Length > 1)
                    {
                        throw new CoconaException($"The parameter set '{paramSetType.FullName}' has two or more constructors.");
                    }

                    if (ctors[0].GetParameters().Length == 0)
                    {
                        // Parameter-less constructor.
                        var propsOrFields = paramSetType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                            .Where(x => (x is PropertyInfo { CanWrite: true }) || (x is FieldInfo))
                            .ToArray();

                        var tempInstance = default(object);
                        var builder2 = builder.AddParameterSet(attrs, methodParam.ParameterType, methodParam.Name);
                        foreach (var propOrField in propsOrFields)
                        {
                            var type = propOrField switch
                            {
                                PropertyInfo propInfo => propInfo.PropertyType,
                                FieldInfo fieldInfo => fieldInfo.FieldType,
                                _ => throw new InvalidOperationException(),
                            };
                            Action<object, object?> setter = propOrField switch
                            {
                                PropertyInfo propInfo => (x, y) => propInfo.SetValue(x, y),
                                FieldInfo fieldInfo => (x, y) => fieldInfo.SetValue(x, y),
                                _ => throw new InvalidOperationException(),
                            };
                            Func<object, object?> getter = propOrField switch
                            {
                                PropertyInfo propInfo => (x) => propInfo.GetValue(x),
                                FieldInfo fieldInfo => (x) => fieldInfo.GetValue(x),
                                _ => throw new InvalidOperationException(),
                            };
                            var attrs2 = new CommandParameterAttributeSet(propOrField.GetCustomAttributes(typeof(Attribute), true));
                            var defaultValue2 = CoconaDefaultValue.None;
                            if (propOrField.GetCustomAttribute<HasDefaultValueAttribute>() is not null)
                            {
                                tempInstance ??= Activator.CreateInstance(paramSetType)!;
                                defaultValue2 = new CoconaDefaultValue(getter(tempInstance));
                            }

                            if (attrs2.Argument is not null)
                            {
                                builder2.AddArgument(attrs2, type, propOrField.Name, defaultValue2, setter);
                            }
                            else if (attrs2.Ignore is not null)
                            {
                                // Skip
                            }
                            else if (attrs2.FromService is not null)
                            {
                                builder2.AddFromService(type, propOrField.Name, setter);
                            }
                            else
                            {
                                builder2.AddOption(attrs2, type, propOrField.Name, defaultValue2, setter);
                            }
                        }

                        builder2.BuildAndAdd();
                    }
                    else
                    {
                        // Parameterized constructor.
                        var builder2 = builder.CreateBuilderForParameterizedParameterSet(attrs, methodParam.ParameterType, methodParam.Name);
                        CollectParameters(ctors[0], ctors[0].GetParameters(), builder2, isPrimaryCommand, isSingleCommand);
                        builder.AddParameterizedParameterSet(attrs, methodParam.ParameterType, methodParam.Name, builder2);
                    }

                    continue;
                }

                if (attrs.Argument != null)
                {
                    if (!isSingleCommand && isPrimaryCommand) throw new CoconaException("A primary command with multiple commands cannot handle/have any arguments.");

                    builder.AddArgument(attrs, methodParam.ParameterType, methodParam.Name, defaultValue);
                    continue;
                }

                if (attrs.FromService != null)
                {
                    builder.AddFromService(methodParam.ParameterType, methodParam.Name);
                    continue;
                }

                // If a parameter has no OptionAttribute, it treated as option.
                {
                    builder.AddOption(attrs, methodParam.ParameterType, methodParam.Name, defaultValue);
                    continue;
                }
            }
        }

        private CommandMethodDescriptor GetCommandMethodDescriptor(MethodInfo methodInfo)
        {
            var commandAttr = default(CommandAttribute);
            var isHidden = false;
            var isPrimaryCommand = false;
            var isIgnoreUnknownOptions = false;
            var optionLikeCommands = new List<OptionLikeCommandAttribute>();
            var commandMethodForwardedToAttr = default(CommandMethodForwardedToAttribute);

            foreach (var attr in methodInfo.GetCustomAttributes(true))
            {
                switch (attr)
                {
                    case CommandAttribute command:
                        commandAttr = command;
                        break;
                    case HiddenAttribute _:
                        isHidden = true;
                        break;
                    case PrimaryCommandAttribute _:
                        isPrimaryCommand = true;
                        break;
                    case IgnoreUnknownOptionsAttribute _:
                        isIgnoreUnknownOptions = true;
                        break;
                    case OptionLikeCommandAttribute optionLikeCommand:
                        optionLikeCommands.Add(optionLikeCommand);
                        break;
                    case CommandMethodForwardedToAttribute commandMethodForwardedTo:
                        commandMethodForwardedToAttr = commandMethodForwardedTo;
                        break;
                }
            }

            if (methodInfo.DeclaringType is null)
            {
                throw new CoconaException($"The method {methodInfo.Name} doesn't have a declaring type.");
            }
            isIgnoreUnknownOptions |= methodInfo.DeclaringType.GetCustomAttribute<IgnoreUnknownOptionsAttribute>() != null;

            return new CommandMethodDescriptor(commandAttr, isHidden, isPrimaryCommand, isIgnoreUnknownOptions, optionLikeCommands, commandMethodForwardedToAttr);
        }

        private class CommandParameterAttributeSet
        {
            public IReadOnlyList<Attribute> Attributes { get; }
            public IgnoreAttribute? Ignore { get; }
            public OptionAttribute? Option { get; }
            public ArgumentAttribute? Argument { get; }
            public FromServiceAttribute? FromService { get; }
            public HiddenAttribute? Hidden { get; }

            public CommandParameterAttributeSet(object[] attrs)
            {
                (Ignore, Option, Argument, FromService, Hidden)
                    = AttributeHelper.GetAttributes<IgnoreAttribute, OptionAttribute, ArgumentAttribute, FromServiceAttribute, HiddenAttribute>(attrs);
                Attributes = attrs.OfType<Attribute>().ToArray();
            }
        }

        private class CommandDescriptorBuilder
        {
            private readonly Dictionary<string, CommandOptionDescriptor> _allOptions;
            private readonly HashSet<char> _allOptionShortNames;
            private readonly List<ICommandParameterDescriptor> _parameters = new List<ICommandParameterDescriptor>();
            private readonly List<CommandArgumentDescriptor> _arguments;
            private readonly bool _enableConvertOptionNameToLowerCase;
            private readonly bool _enableConvertArgumentNameToLowerCase;

            private int _defaultArgOrder = 0;

            public IReadOnlyDictionary<string, CommandOptionDescriptor> AllOptions => _allOptions;

            public CommandDescriptorBuilder(
                bool enableConvertOptionNameToLowerCase,
                bool enableConvertArgumentNameToLowerCase,
                Dictionary<string, CommandOptionDescriptor>? allOptions = default,
                HashSet<char>? allOptionShortNames = default,
                List<CommandArgumentDescriptor>? arguments = default
            )
            {
                _enableConvertOptionNameToLowerCase = enableConvertOptionNameToLowerCase;
                _enableConvertArgumentNameToLowerCase = enableConvertArgumentNameToLowerCase;
                _allOptions = allOptions ?? new Dictionary<string, CommandOptionDescriptor>(StringComparer.OrdinalIgnoreCase);
                _allOptionShortNames = allOptionShortNames ?? new HashSet<char>();
                _arguments = arguments ?? new List<CommandArgumentDescriptor>();
            }

            public CommandServiceParameterDescriptor CreateFromService(Type type, string name)
            {
                return new CommandServiceParameterDescriptor(type, name);
            }

            public void AddFromService(Type type, string name)
            {
                _parameters.Add(CreateFromService(type, name));
            }

            public void AddIgnore(Type type, string name, object? defaultValue)
            {
                var ignoreParamDescriptor = new CommandIgnoredParameterDescriptor(type, name, defaultValue);
                _parameters.Add(ignoreParamDescriptor);
            }

            public CommandArgumentDescriptor CreateArgument(CommandParameterAttributeSet attrSet, Type type,
                string name, CoconaDefaultValue defaultValue)
            {
                if (attrSet.Argument is null) throw new InvalidOperationException("ArgumentAttribute must not be null.");

                var argName = attrSet.Argument.Name ?? name;
                var argDesc = attrSet.Argument.Description ?? string.Empty;
                var argOrder = attrSet.Argument.Order != 0 ? attrSet.Argument.Order : _defaultArgOrder;

                if (_enableConvertArgumentNameToLowerCase) argName = ToCommandCase(argName);

                return new CommandArgumentDescriptor(
                    type,
                    argName,
                    argOrder,
                    argDesc,
                    defaultValue,
                    attrSet.Attributes);
            }

            public void AddArgument(CommandParameterAttributeSet attrSet, Type type, string name, CoconaDefaultValue defaultValue)
            {
                var commandArgDescriptor = CreateArgument(attrSet, type, name, defaultValue);

                _defaultArgOrder++;
                _parameters.Add(commandArgDescriptor);
                _arguments.Add(commandArgDescriptor);
            }

            public CommandOptionDescriptor CreateOption(CommandParameterAttributeSet attrSet, Type type, string name, CoconaDefaultValue defaultValue)
            {
                var optionName = attrSet.Option?.Name ?? name;
                var optionDesc = attrSet.Option?.Description ?? string.Empty;
                var optionShortNames = attrSet.Option?.ShortNames ?? Array.Empty<char>();
                var optionValueName = attrSet.Option?.ValueName ?? (DynamicListHelper.IsArrayOrEnumerableLike(type) ? DynamicListHelper.GetElementType(type) : type).Name;
                var optionIsHidden = attrSet.Hidden != null;

                if (_enableConvertOptionNameToLowerCase) optionName = ToCommandCase(optionName);

                // If the option type is bool, the option has always default value (false).
                if (!defaultValue.HasValue && type == typeof(bool))
                {
                    defaultValue = new CoconaDefaultValue(false);
                }

                return new CommandOptionDescriptor(
                    type,
                    optionName,
                    optionShortNames,
                    optionDesc,
                    defaultValue,
                    optionValueName,
                    optionIsHidden ? CommandOptionFlags.Hidden : CommandOptionFlags.None,
                    attrSet.Attributes);
            }

            public void AddOption(CommandParameterAttributeSet attrSet, Type type, string name, CoconaDefaultValue defaultValue)
            {
                var optionDescriptor = CreateOption(attrSet, type, name, defaultValue);

                if (_allOptions.ContainsKey(optionDescriptor.Name))
                    throw new CoconaException($"Option '{optionDescriptor.Name}' is already exists.");
                if (_allOptionShortNames.Count != 0 && optionDescriptor.ShortName.Count != 0 && _allOptionShortNames.IsSupersetOf(optionDescriptor.ShortName))
                    throw new CoconaException($"Short name option '{string.Join(",", optionDescriptor.ShortName)}' is already exists.");

                _allOptions.Add(optionDescriptor.Name, optionDescriptor);
                _allOptionShortNames.UnionWith(optionDescriptor.ShortName);

                _parameters.Add(optionDescriptor);
            }

            public CommandDescriptorBuilder CreateBuilderForParameterizedParameterSet(CommandParameterAttributeSet attrSet, Type type, string name)
            {
                if (attrSet.Option is not null || attrSet.Argument is not null || attrSet.FromService is not null)
                {
                    throw new CoconaException($"parameter set '{name}' must not be marked as Option, Argument, FromService");
                }

                // Options and Arguments are shared between current builder and nested-builder.
                return new CommandDescriptorBuilder(_enableConvertOptionNameToLowerCase, _enableConvertArgumentNameToLowerCase, _allOptions, _allOptionShortNames, _arguments);
            }

            public void AddParameterizedParameterSet(CommandParameterAttributeSet attrSet, Type type, string name, CommandDescriptorBuilder nestedBuilder)
            {
                if (attrSet.Option is not null || attrSet.Argument is not null || attrSet.FromService is not null)
                {
                    throw new CoconaException($"parameter set '{name}' must not be marked as Option, Argument, FromService");
                }

                var (parameters, _, _) = nestedBuilder.Build();
                _parameters.Add(new CommandParameterizedParameterSetDescriptor(type, name, attrSet.Attributes, parameters));
            }

            public ParameterSetBuilder AddParameterSet(CommandParameterAttributeSet attrSet, Type type, string name)
            {
                if (attrSet.Option is not null || attrSet.Argument is not null || attrSet.FromService is not null)
                {
                    throw new CoconaException($"parameter set '{name}' must not be marked as Option, Argument, FromService");
                }
                return new ParameterSetBuilder(this, attrSet, type, name);
            }

            public class ParameterSetBuilder
            {
                private readonly CommandDescriptorBuilder _parent;
                private readonly CommandParameterAttributeSet _attrSet;
                private readonly Type _type;
                private readonly string _name;
                private readonly List<CommandParameterSetMemberDescriptor> _memberDescriptors;

                public ParameterSetBuilder(CommandDescriptorBuilder parent, CommandParameterAttributeSet attrSet, Type type, string name)
                {
                    _parent = parent;
                    _memberDescriptors = new List<CommandParameterSetMemberDescriptor>();

                    _attrSet = attrSet;
                    _type = type;
                    _name = name;
                }

                public void AddArgument(CommandParameterAttributeSet attrSet, Type type, string name, CoconaDefaultValue defaultValue, Action<object, object?> setter)
                {
                    var argumentDescriptor = _parent.CreateArgument(attrSet, type, name, defaultValue);
                    _parent._defaultArgOrder++;

                    _parent._arguments.Add(argumentDescriptor);
                    _memberDescriptors.Add(new CommandParameterSetMemberDescriptor(argumentDescriptor, setter));
                }

                public void AddOption(CommandParameterAttributeSet attrSet, Type type, string name, CoconaDefaultValue defaultValue, Action<object, object?> setter)
                {
                    var optionDescriptor = _parent.CreateOption(attrSet, type, name, defaultValue);

                    if (_parent._allOptions.ContainsKey(optionDescriptor.Name))
                        throw new CoconaException($"Option '{optionDescriptor.Name}' is already exists.");
                    if (_parent._allOptionShortNames.Count != 0 && optionDescriptor.ShortName.Count != 0 && _parent._allOptionShortNames.IsSupersetOf(optionDescriptor.ShortName))
                        throw new CoconaException($"Short name option '{string.Join(",", optionDescriptor.ShortName)}' is already exists.");

                    _parent._allOptions.Add(optionDescriptor.Name, optionDescriptor);
                    _parent._allOptionShortNames.UnionWith(optionDescriptor.ShortName);

                    _memberDescriptors.Add(new CommandParameterSetMemberDescriptor(optionDescriptor, setter));
                }

                public void AddFromService(Type type, string name, Action<object, object?> setter)
                {
                    _memberDescriptors.Add(new CommandParameterSetMemberDescriptor(_parent.CreateFromService(type, name), setter));
                }

                public void BuildAndAdd()
                {
                    var parameterSetDesc = new CommandParameterSetDescriptor(_type, _name, _attrSet.Attributes, _memberDescriptors);
                    _parent._parameters.Add(parameterSetDesc);
                }
            }

            public (IReadOnlyList<ICommandParameterDescriptor> Parameters, IReadOnlyList<CommandOptionDescriptor>
                Options, IReadOnlyList<CommandArgumentDescriptor> Arguments) Build()
            {
                return (_parameters, _allOptions.Values.ToArray(), _arguments);
            }
        }

        private readonly struct CommandMethodDescriptor
        {
            public CommandAttribute? CommandAttribute { get; }
            public bool IsHidden { get; }
            public bool IsPrimaryCommand { get; }
            public bool IsIgnoreUnknownOptions { get; }
            public IReadOnlyList<OptionLikeCommandAttribute> OptionLikeCommands { get; }
            public CommandMethodForwardedToAttribute? CommandMethodForwardedTo { get; }

            public CommandMethodDescriptor(CommandAttribute? commandAttr, bool isHidden, bool isPrimaryCommand, bool isIgnoreUnknownOptions, IReadOnlyList<OptionLikeCommandAttribute> optionLikeCommands, CommandMethodForwardedToAttribute? commandMethodForwardedTo)
            {
                CommandAttribute = commandAttr;
                IsHidden = isHidden;
                IsPrimaryCommand = isPrimaryCommand;
                IsIgnoreUnknownOptions = isIgnoreUnknownOptions;
                OptionLikeCommands = optionLikeCommands;
                CommandMethodForwardedTo = commandMethodForwardedTo;
            }
        }

        public static string ToCommandCase(string value)
        {
            var sb = new StringBuilder(value.Length);
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (IsInRange(c, 'A', 'Z'))
                {
                    if (i > 0 && IsInRange(value[i - 1], 'a', 'z'))
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInRange(char c, char min, char max) => (uint)(c - min) <= (uint)(max - min);
    }
}
