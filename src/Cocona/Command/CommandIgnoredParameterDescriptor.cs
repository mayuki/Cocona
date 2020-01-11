using System;
using System.Diagnostics;

namespace Cocona.Command
{
    [DebuggerDisplay("Ignored: {ArgumentType.FullName,nq}; DefaultValue={DefaultValue,nq})")]
    public class CommandIgnoredParameterDescriptor : CommandParameterDescriptor
    {
        public Type ParameterType { get; }
        public object? DefaultValue { get; }

        public CommandIgnoredParameterDescriptor(Type parameterType, object? defaultValue)
        {
            ParameterType = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
            DefaultValue = defaultValue;
        }
    }
}
