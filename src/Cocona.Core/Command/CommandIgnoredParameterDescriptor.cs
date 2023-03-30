using System.Diagnostics;

namespace Cocona.Command
{
    [DebuggerDisplay("Ignored: {ParameterType.FullName,nq}; DefaultValue={DefaultValue,nq})")]
    public class CommandIgnoredParameterDescriptor : ICommandParameterDescriptor
    {
        public string Name { get; }
        public Type ParameterType { get; }
        public object? DefaultValue { get; }
        public IReadOnlyList<Attribute> ParameterAttributes { get; } = Array.Empty<Attribute>();

        public CommandIgnoredParameterDescriptor(Type parameterType, string name, object? defaultValue)
        {
            ParameterType = parameterType ?? throw new ArgumentNullException(nameof(parameterType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DefaultValue = defaultValue;
        }
    }
}
