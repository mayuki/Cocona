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

        public CoconaCommandProvider(Type[] targetTypes)
        {
            _targetTypes = targetTypes ?? throw new ArgumentNullException(nameof(targetTypes));
        }

        public CommandCollection GetCommandCollection()
        {
            var methods = _targetTypes
                .Where(x => x.GetCustomAttribute<IgnoreAttribute>() == null) // class-level ignore
                .Where(x => !x.IsAbstract && (!x.IsGenericType || x.IsConstructedGenericType)) // non-abstract, non-generic, closed-generic
                .SelectMany(xs => xs.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                .Where(x => x.DeclaringType != typeof(object) && (x.IsPublic || x.GetCustomAttributes<CommandAttribute>(inherit: true).Any())) // is not System.Object && (public || has-command attr)
                .Where(x => x.GetCustomAttribute<IgnoreAttribute>() == null); // method-level ignore

            return new CommandCollection(methods.Select(x => CreateCommand(x)).ToArray());
        }

        public CommandDescriptor CreateCommand(MethodInfo methodInfo)
        {
            var commandAttr = methodInfo.GetCustomAttribute<CommandAttribute>();
            var commandName = commandAttr?.Name ?? methodInfo.Name;
            var description = commandAttr?.Description ?? string.Empty;
            var aliases = commandAttr?.Aliases ?? Array.Empty<string>();

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
                    return (CommandParameterDescriptor)new CommandOptionDescriptor(x.ParameterType, optionName, optionShortNames, optionDesc, defaultValue);
                })
                .ToArray();

            return new CommandDescriptor(
                methodInfo,
                commandName,
                aliases,
                description,
                parameters
            );
        }
    }
}
