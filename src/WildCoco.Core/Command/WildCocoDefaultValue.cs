using System.Diagnostics;

namespace WildCoco.Command;

[DebuggerDisplay("DefaultValue: {ToString(),nq}")]
public readonly struct WildCocoDefaultValue
{
    public static WildCocoDefaultValue None { get; } = new WildCocoDefaultValue();

    public bool HasValue { get; }
    public object? Value { get; }

    public WildCocoDefaultValue(object? value)
    {
        HasValue = true;
        Value = value;
    }

    public override string ToString()
    {
        return $"DefaultValue: {(HasValue ? Value?.ToString() ?? "(null)" : "None")}";
    }
}
