using WildCoco.Command;
using WildCoco.CommandLine;

namespace WildCoco.Filters;

public class WildCocoCommandExecutingContext
{
    public ParsedCommandLine ParsedCommandLine { get; }
    public CommandDescriptor Command { get; }
    public object? CommandTarget { get; }

    public WildCocoCommandExecutingContext(CommandDescriptor command, ParsedCommandLine parsedCommandLine, object? commandTarget)
    {
        Command = command ?? throw new ArgumentNullException(nameof(command));
        ParsedCommandLine = parsedCommandLine ?? throw new ArgumentNullException(nameof(parsedCommandLine));
        CommandTarget = commandTarget;
    }
}
