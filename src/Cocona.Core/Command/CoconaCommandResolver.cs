using System;
using System.Collections.Generic;
using System.Linq;
using Cocona.Command.Dispatcher;
using Cocona.CommandLine;

namespace Cocona.Command
{
    public class CoconaCommandResolver : ICoconaCommandResolver
    {
        private readonly ICoconaCommandProvider _commandProvider;
        private readonly ICoconaCommandLineParser _commandLineParser;
        private readonly ICoconaCommandMatcher _commandMatcher;

        public CoconaCommandResolver(
            ICoconaCommandProvider commandProvider,
            ICoconaCommandLineParser commandLineParser,
            ICoconaCommandMatcher commandMatcher
        )
        {
            _commandProvider = commandProvider;
            _commandLineParser = commandLineParser;
            _commandMatcher = commandMatcher;
        }

        public CommandResolverResult ParseAndResolve(IReadOnlyList<string> args)
        {
            var commandCollection = _commandProvider.GetCommandCollection();
            var subCommandStack = new List<CommandDescriptor>();

            Retry:
            var matchedCommand = default(CommandDescriptor);
            if (commandCollection.All.Count == 1 && !commandCollection.All[0].Flags.HasFlag(CommandFlags.SubCommandsEntryPoint))
            {
                // single-command style
                matchedCommand = commandCollection.All[0];
            }
            else if (commandCollection.All.Count > 0)
            {
                // multi-commands hosted style
                if (_commandLineParser.TryGetCommandName(args, out var commandName))
                {
                    if (!_commandMatcher.TryGetCommand(commandName, commandCollection, out matchedCommand))
                    {
                        throw new CommandNotFoundException(
                            commandName,
                            commandCollection,
                            $"The specified command '{commandName}' was not found."
                        );
                    }

                    // NOTE: Skip a first argument that is command name.
                    args = args.Skip(1).ToArray();

                    // If the command has nested sub-commands, try to restart parse command.
                    if (matchedCommand.SubCommands != null)
                    {
                        commandCollection = matchedCommand.SubCommands;
                        subCommandStack.Add(matchedCommand);
                        goto Retry;
                    }
                }
                else
                {
                    // Use default command (NOTE: The default command must have no argument.)
                    matchedCommand = commandCollection.Primary ?? throw new CommandNotFoundException("", commandCollection, "A primary command was not found.");
                }
            }

            // Found a command and dispatch.
            if (matchedCommand != null)
            {
                // resolve command overload
                if (matchedCommand.Overloads.Any())
                {
                    // Try parse command-line for overload resolution by options.
                    var preParsedCommandLine = _commandLineParser.ParseCommand(args, matchedCommand.Options.OfType<ICommandOptionDescriptor>().Concat(matchedCommand.OptionLikeCommands).ToArray(), matchedCommand.Arguments);
                    matchedCommand = _commandMatcher.ResolveOverload(matchedCommand, preParsedCommandLine);
                }

                var parsedCommandLine = _commandLineParser.ParseCommand(args, matchedCommand.Options.OfType<ICommandOptionDescriptor>().Concat(matchedCommand.OptionLikeCommands).ToArray(), matchedCommand.Arguments);

                // OptionLikeCommand
                if (parsedCommandLine.Options.FirstOrDefault(x => x.Option is CommandOptionLikeCommandDescriptor) is var commandOption &&
                    commandOption.Option is CommandOptionLikeCommandDescriptor optionLikeCommand)
                {
                    subCommandStack.Add(matchedCommand);
                    matchedCommand = optionLikeCommand.Command;
                    parsedCommandLine = _commandLineParser.ParseCommand(args.Skip(commandOption.Position + 1).ToArray(), matchedCommand.Options, matchedCommand.Arguments);
                }

                return new CommandResolverResult(true, commandCollection, parsedCommandLine, matchedCommand, subCommandStack);
            }

            return new CommandResolverResult(false, commandCollection, null, null, null);
        }
    }
}
