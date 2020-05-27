using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OptionLikeCommandAttribute : Attribute
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
    }
}
