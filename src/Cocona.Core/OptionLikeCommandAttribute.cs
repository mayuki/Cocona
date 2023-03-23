using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cocona.Builder;
using Cocona.Builder.Metadata;
using Cocona.Resources;

namespace Cocona
{
    /// <summary>
    /// Specifies that the command has the option-like command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OptionLikeCommandAttribute : Attribute, IOptionLikeCommandMetadata
    {
        public string OptionName { get; }
        public IReadOnlyList<char> ShortNames { get; }
        public Type CommandType { get; }
        public string CommandMethodName { get; }
        public string? Description { get; }

        public OptionLikeCommandAttribute(string optionName, char[] shortNames, Type commandType, string commandMethodName, string? description = null)
        {
            OptionName = optionName ?? throw new ArgumentNullException(nameof(optionName));
            ShortNames = shortNames ?? throw new ArgumentNullException(nameof(shortNames));
            CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
            CommandMethodName = commandMethodName ?? throw new ArgumentNullException(nameof(commandMethodName));
            Description = description;
        }

        public ICommandData GetCommandData()
        {
            var methodInfo = CommandType.GetMethod(CommandMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo is null)
            {
                throw new InvalidOperationException(string.Format(Strings.OptionLikeCommand_MethodNotFound, CommandMethodName, CommandType));
            }

            return new DelegateCommandData(methodInfo, null, new [] { new CommandNameMetadata(OptionName) }
                .Concat(Description is { } description ? new object[] { new CommandDescriptionMetadata(description) } : new object[0])
                .Concat(methodInfo.GetCustomAttributes(inherit: true)).ToArray());
        }
    }
}
