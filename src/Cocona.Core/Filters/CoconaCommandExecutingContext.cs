using Cocona.Command;
using Cocona.CommandLine;

namespace Cocona.Filters;

public class CoconaCommandExecutingContext
{
    public ParsedCommandLine ParsedCommandLine { get; }
    public CommandDescriptor Command { get; }
    public object? CommandTarget { get; }

    public CoconaCommandExecutingContext(CommandDescriptor command, ParsedCommandLine parsedCommandLine, object? commandTarget)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
        ParsedCommandLine = parsedCommandLine ?? throw new ArgumentNullException(nameof(parsedCommandLine));
        CommandTarget = commandTarget;
    }
}