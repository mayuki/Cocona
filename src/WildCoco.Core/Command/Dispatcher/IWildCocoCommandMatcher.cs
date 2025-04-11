using WildCoco.CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace WildCoco.Command.Dispatcher;

public interface IWildCocoCommandMatcher
{
    bool TryGetCommand(string commandName, CommandCollection commandCollection, [NotNullWhen(true)] out CommandDescriptor? command);
    CommandDescriptor ResolveOverload(CommandDescriptor command, ParsedCommandLine parsedCommandLine);
}
