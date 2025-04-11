using WildCoco.Command;
using System.Diagnostics.CodeAnalysis;

namespace WildCoco.CommandLine;

/// <summary>
/// A command-line arguments parser for WildCoco.
/// </summary>
public interface IWildCocoCommandLineParser
{
    ParsedCommandLine ParseCommand(IReadOnlyList<string> args, IReadOnlyList<ICommandOptionDescriptor> options, IReadOnlyList<CommandArgumentDescriptor> arguments);
    bool TryGetCommandName(IReadOnlyList<string> args, [NotNullWhen(true)] out string? commandName);
}
