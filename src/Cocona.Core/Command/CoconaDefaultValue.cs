using System.Diagnostics;

namespace Cocona.Command
{
    [DebuggerDisplay("DefaultValue: {ToString(),nq}")]
    public readonly struct CoconaDefaultValue
    {
        public static CoconaDefaultValue None { get; } = new CoconaDefaultValue();

        public bool HasValue { get; }
        public object Value { get; }

        public CoconaDefaultValue(object value)
        {
            HasValue = true;
            Value = value;
        }

        public override string ToString()
        {
            return $"DefaultValue: {(HasValue ? Value?.ToString() ?? "(null)" : "None")}";
        }
    }
}
