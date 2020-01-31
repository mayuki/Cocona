using Cocona.CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace Cocona.Command.Dispatcher
{
    public interface ICoconaCommandMatcher
    {
        bool TryGetCommand(string commandName, CommandCollection commandCollection, [NotNullWhen(true)] out CommandDescriptor? command);
        CommandDescriptor ResolveOverload(CommandDescriptor command, ParsedCommandLine parsedCommandLine);
    }
}
