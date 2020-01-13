using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CommandOverloadAttribute : Attribute
    {
        public string TargetCommand { get; }
        public string OptionName { get; }
        public string? OptionValue { get; }
        public StringComparison StringComparison { get; }

        public CommandOverloadAttribute(string targetCommand, string optionName, string? optionValue = null, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            TargetCommand = targetCommand ?? throw new ArgumentNullException(nameof(targetCommand));
            OptionName = optionName ?? throw new ArgumentNullException(nameof(optionName));
            OptionValue = optionValue;
            StringComparison = stringComparison;
        }
    }
}
