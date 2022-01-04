using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Cocona.Builder;
using Cocona.Builder.Metadata;

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

        public OptionLikeCommandAttribute(string optionName, char[] shortNames, Type commandType, string commandMethodName)
        {
            OptionName = optionName ?? throw new ArgumentNullException(nameof(optionName));
            ShortNames = shortNames ?? throw new ArgumentNullException(nameof(shortNames));
            CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
            CommandMethodName = commandMethodName ?? throw new ArgumentNullException(nameof(commandMethodName));
        }

        public ICommandData GetCommandData()
        {
            var methodInfo = CommandType.GetMethod(CommandMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo is null)
            {
                throw new InvalidOperationException($"A method of option-like command '{CommandMethodName}' was not found in '{CommandType}'");
            }

            return new DelegateCommandData(methodInfo, null, new [] { new CommandNameMetadata(OptionName) }.Concat(methodInfo.GetCustomAttributes(inherit: true)).ToArray());
        }
    }
}
