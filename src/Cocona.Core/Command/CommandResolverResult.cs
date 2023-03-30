using Cocona.CommandLine;

namespace Cocona.Command
{
    public readonly struct CommandResolverResult
    {
        public bool Success { get; }

        public CommandCollection CommandCollection { get; }
        public ParsedCommandLine? ParsedCommandLine { get; }
        public CommandDescriptor? MatchedCommand { get; }
        public IReadOnlyList<CommandDescriptor>? SubCommandStack { get; }

        public CommandResolverResult(
            bool success,
            CommandCollection commandCollection,
            ParsedCommandLine? parsedCommandLine,
            CommandDescriptor? matchedCommand,
            IReadOnlyList<CommandDescriptor>? subCommandStack
        )
        {
            Success = success;
            CommandCollection = commandCollection;
            ParsedCommandLine = parsedCommandLine;
            MatchedCommand = matchedCommand;
            SubCommandStack = subCommandStack;
        }
    }
}
