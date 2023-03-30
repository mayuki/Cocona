using Cocona.CommandLine;

namespace Cocona.Command.Dispatcher
{
    public class CommandDispatchContext
    {
        public CommandDescriptor Command { get;}
        
        public object? CommandTarget { get; }

        public ParsedCommandLine ParsedCommandLine { get; }

        public CancellationToken CancellationToken { get; }

        public CommandDispatchContext(CommandDescriptor command, ParsedCommandLine parsedCommandLine, object? commandTarget, CancellationToken cancellationToken)
        {
            Command = command;
            ParsedCommandLine = parsedCommandLine;
            CommandTarget = commandTarget;
            CancellationToken = cancellationToken;
        }
    }
}
