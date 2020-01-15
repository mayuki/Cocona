using Cocona.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cocona.Command
{
    public class CoconaCommandProvider : ICoconaCommandProvider
    {
        private readonly Type[] _targetTypes;
        private static readonly Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>> _emptyOverloads = new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>();
        private readonly Lazy<CommandCollection> _commandCollection;

        public CoconaCommandProvider(Type[] targetTypes)
        {
            _targetTypes = targetTypes ?? throw new ArgumentNullException(nameof(targetTypes));
            _commandCollection = new Lazy<CommandCollection>(GetCommandCollectionCore);
        }

        public CommandCollection GetCommandCollection()
            => _commandCollection.Value;

        private CommandCollection GetCommandCollectionCore()
        {
            var candidateMethods = _targetTypes
                .Where(x => x.GetCustomAttribute<IgnoreAttribute>() == null) // class-level ignore
                .Where(x => !x.IsAbstract && (!x.IsGenericType || x.IsConstructedGenericType)) // non-abstract, non-generic, closed-generic
                .SelectMany(xs => xs.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                .Where(x => !x.IsSpecialName && x.DeclaringType != typeof(object) && (x.IsPublic || x.GetCustomAttributes<CommandAttribute>(inherit: true).Any())) // not-property && not-System.Object && (public || has-command attr)
                .Where(x => x.GetCustomAttribute<IgnoreAttribute>() == null); // method-level ignore

            var commandMethods = new List<MethodInfo>();
            var overloadCommandMethods = new Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>>();

            foreach (var method in candidateMethods)
            {
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

            var hasMultipleCommand = commandMethods.Count > 1;
            var commandNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var commands = new List<CommandDescriptor>();
            foreach (var command in commandMethods.Select(x => CreateCommand(x, !hasMultipleCommand, overloadCommandMethods)))
            {
                if (commandNames.Contains(command.Name))
                {
                    throw new CoconaException($"Command '{command.Name}' has already exists. (Method: {command.Method.Name})");
                }
                commandNames.Add(command.Name);

                if (command.Aliases.Any())
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

        public CommandDescriptor CreateCommand(MethodInfo methodInfo, bool isSingleCommand, Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>> overloadCommandMethods)
        {
            ThrowHelper.ArgumentNull(methodInfo, nameof(methodInfo));

            var commandAttr = methodInfo.GetCustomAttribute<CommandAttribute>();
            var commandName = commandAttr?.Name ?? methodInfo.Name;
            var description = commandAttr?.Description ?? string.Empty;
            var aliases = commandAttr?.Aliases ?? Array.Empty<string>();

            var isPrimaryCommand = methodInfo.GetCustomAttribute<PrimaryCommandAttribute>() != null;

            var allOptions = new Dictionary<string, CommandOptionDescriptor>(StringComparer.OrdinalIgnoreCase);
            var allOptionShortNames = new HashSet<char>();

            var defaultArgOrder = 0;
            var parameters = methodInfo.GetParameters()
                .Select((x, i) =>
                {
                    var defaultValue = x.HasDefaultValue ? new CoconaDefaultValue(x.DefaultValue) : CoconaDefaultValue.None;

                    var ignoreAttr = x.GetCustomAttribute<IgnoreAttribute>();
                    if (ignoreAttr != null)
                    {
                        return (CommandParameterDescriptor)new CommandIgnoredParameterDescriptor(
                            x.ParameterType,
                            x.HasDefaultValue
                                ? x.DefaultValue
                                : x.ParameterType.IsValueType
                                    ? Activator.CreateInstance(x.ParameterType)
                                    : null
                        );
                    }

                    var argumentAttr = x.GetCustomAttribute<ArgumentAttribute>();
                    if (argumentAttr != null)
                    {
                        if (!isSingleCommand && isPrimaryCommand) throw new CoconaException("A primary command with multiple commands cannot handle/have any arguments.");

                        var argName = argumentAttr.Name ?? x.Name;
                        var argDesc = argumentAttr.Description ?? string.Empty;
                        var argOrder = argumentAttr.Order != 0 ? argumentAttr.Order : defaultArgOrder;

                        defaultArgOrder++;

                        return (CommandParameterDescriptor)new CommandArgumentDescriptor(x.ParameterType, argName, argOrder, argDesc, defaultValue);
                    }

                    var fromServiceAttr = x.GetCustomAttribute<FromServiceAttribute>();
                    if (fromServiceAttr != null)
                    {
                        return (CommandParameterDescriptor)new CommandServiceParameterDescriptor(x.ParameterType);
                    }

                    var optionAttr = x.GetCustomAttribute<OptionAttribute>();
                    var optionName = optionAttr?.Name ?? x.Name;
                    var optionDesc = optionAttr?.Description ?? string.Empty;
                    var optionShortNames = optionAttr?.ShortNames ?? Array.Empty<char>();
                    var optionValueName = optionAttr?.ValueName ?? x.ParameterType.Name;

                    // If the option type is bool, the option has always default value (false).
                    if (!defaultValue.HasValue && x.ParameterType == typeof(bool))
                    {
                        defaultValue = new CoconaDefaultValue(false);
                    }

                    if (allOptions.ContainsKey(optionName))
                        throw new CoconaException($"Option '{optionName}' is already exists.");
                    if (allOptionShortNames.Any() && optionShortNames.Any() && allOptionShortNames.IsSupersetOf(optionShortNames))
                        throw new CoconaException($"Short name option '{string.Join(",", optionShortNames)}' is already exists.");

                    var option = new CommandOptionDescriptor(x.ParameterType, optionName, optionShortNames, optionDesc, defaultValue, optionValueName);
                    allOptions.Add(optionName, option);
                    allOptionShortNames.UnionWith(optionShortNames);

                    return (CommandParameterDescriptor)option;
                })
                .ToArray();

            var options = parameters.OfType<CommandOptionDescriptor>().ToList();
            var arguments = parameters.OfType<CommandArgumentDescriptor>().ToArray();

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

            return new CommandDescriptor(
                methodInfo,
                commandName,
                aliases,
                description,
                parameters,
                options,
                arguments,
                overloadDescriptors.ToArray(),
                isSingleCommand || isPrimaryCommand
            );
        }
    }

    public class CommandOverloadDescriptor
    {
        public CommandOptionDescriptor Option { get; }
        public string? Value { get; }
        public CommandDescriptor Command { get; }
        public IEqualityComparer<string> Comparer { get; }

        public CommandOverloadDescriptor(CommandOptionDescriptor option, string? value, CommandDescriptor command, IEqualityComparer<string>? comparer)
        {
            Option = option ?? throw new ArgumentNullException(nameof(option));
            Value = value;
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Comparer = comparer ?? StringComparer.OrdinalIgnoreCase;
        }
    }
}
