using System.Diagnostics;

namespace WildCoco.CommandLine;

[DebuggerDisplay("CommandArgument: {Value,nq}")]
public readonly struct CommandArgument
{
    public string Value { get; }
    public int Position { get; }

    public CommandArgument(string value, int position)
    {
        Value = value;
        Position = position;
    }

    public override string ToString()
    {
        return $"CommandArgument: {Value}";
    }
}
