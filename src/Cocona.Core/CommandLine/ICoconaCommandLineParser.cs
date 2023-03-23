using Cocona.Command;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cocona.CommandLine
{
    /// <summary>
    /// A command-line arguments parser for Cocona.
    /// </summary>
    public interface ICoconaCommandLineParser
    {
        ParsedCommandLine ParseCommand(IReadOnlyList<string> args, IReadOnlyList<ICommandOptionDescriptor> options, IReadOnlyList<CommandArgumentDescriptor> arguments);
        bool TryGetCommandName(IReadOnlyList<string> args, [NotNullWhen(true)] out string? commandName);
    }
}
