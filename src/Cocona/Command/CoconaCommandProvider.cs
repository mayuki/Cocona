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

        public CoconaCommandProvider(Type[] targetTypes)
        {
            _targetTypes = targetTypes ?? throw new ArgumentNullException(nameof(targetTypes));
        }

        public CommandCollection GetCommandCollection()
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

            var singleCommand = commandMethods.Count == 1;

            return new CommandCollection(commandMethods.Select(x => CreateCommand(x, singleCommand, overloadCommandMethods)).ToArray());
        }

        public CommandDescriptor CreateCommand(MethodInfo methodInfo, bool isSingleCommand, Dictionary<string, List<(MethodInfo Method, CommandOverloadAttribute Attribute)>> overloadCommandMethods)
        {
            ThrowHelper.ArgumentNull(methodInfo, nameof(methodInfo));

            var commandAttr = methodInfo.GetCustomAttribute<CommandAttribute>();
            var commandName = commandAttr?.Name ?? methodInfo.Name;
            var description = commandAttr?.Description ?? string.Empty;
            var aliases = commandAttr?.Aliases ?? Array.Empty<string>();

            var isPrimaryCommand = methodInfo.GetCustomAttribute<PrimaryCommandAttribute>() != null;

            var allOptions = new Dictionary<string, CommandOptionDescriptor>();
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
