using Cocona.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cocona.CommandLine
{
    /// <summary>
    /// A command-line arguments parser for Cocona.
    /// </summary>
    public interface ICoconaCommandLineParser
    {
        ParsedCommandLine ParseCommand(IReadOnlyList<string> args, IReadOnlyList<CommandOptionDescriptor> options, IReadOnlyList<CommandArgumentDescriptor> arguments);
        bool TryGetCommandName(string[] args, [MaybeNullWhen(false)] out string commandName);
    }
}
