using Cocona.Command;
using System;
using System.Diagnostics;

namespace Cocona.CommandLine
{
    [DebuggerDisplay("CommandOption: {Option.Name,nq}; Value={Value,nq}")]
    public readonly struct CommandOption
    {
        public CommandOptionDescriptor Option { get; }

        public string? Value { get; }

        public CommandOption(CommandOptionDescriptor option, string? value)
        {
            Option = option ?? throw new ArgumentNullException(nameof(option));
            Value = value;
        }

        public override string ToString()
        {
            return $"CommandOption: {Option.Name}; Value={Value}";
        }
    }
}
