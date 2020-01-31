using System.Diagnostics;

namespace Cocona.CommandLine
{
    [DebuggerDisplay("CommandArgument: {Value,nq}")]
    public readonly struct CommandArgument
    {
        public string Value { get; }

        public CommandArgument(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"CommandArgument: {Value}";
        }
    }
}
