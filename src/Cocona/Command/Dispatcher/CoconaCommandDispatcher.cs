using Cocona.Command.BuiltIn;
using Cocona.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher
{
    public class CoconaCommandDispatcher : ICoconaCommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICoconaCommandProvider _commandProvider;
        private readonly ICoconaCommandLineParser _commandLineParser;
        private readonly ICoconaCommandLineArgumentProvider _commandLineArgumentProvider;
        private readonly ICoconaCommandDispatcherPipelineBuilder _dispatcherPipelineBuilder;
        private readonly ICoconaCommandMatcher _commandMatcher;

        public CoconaCommandDispatcher(
            IServiceProvider serviceProvider,
            ICoconaCommandProvider commandProvider,
            ICoconaCommandLineParser commandLineParser,
            ICoconaCommandLineArgumentProvider commandLineArgumentProvider,
            ICoconaCommandDispatcherPipelineBuilder dispatcherPipelineBuilder,
            ICoconaCommandMatcher commandMatcher
        )
        {
            _serviceProvider = serviceProvider;
            _commandProvider = commandProvider;
            _commandLineParser = commandLineParser;
            _commandLineArgumentProvider = commandLineArgumentProvider;
            _dispatcherPipelineBuilder = dispatcherPipelineBuilder;
            _commandMatcher = commandMatcher;
        }

        public ValueTask<int> DispatchAsync()
        {
            var commandCollection = _commandProvider.GetCommandCollection();
            var args = _commandLineArgumentProvider.GetArguments();

            var matchedCommand = default(CommandDescriptor);
            if (commandCollection.All.Count > 1)
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
                }
                else
                {
                    // Use default command (NOTE: The default command must have no argument.)
                    matchedCommand = commandCollection.Primary ?? BuiltInPrimaryCommand.GetCommand(commandCollection);
                }
            }
            else
            {
                // single-command style
                if (commandCollection.All.Any())
                {
                    matchedCommand = commandCollection.All[0];
                }
            }

            // Found a command and dispatch.
            if (matchedCommand != null)
            {
                // resolve command overload
                if (matchedCommand.Overloads.Any())
                {
                    // Try parse command-line for overload resolution by options.
                    var parsedCommandLine = _commandLineParser.ParseCommand(args, matchedCommand.Options, matchedCommand.Arguments);
                    matchedCommand = _commandMatcher.ResolveOverload(matchedCommand, parsedCommandLine);
                }

                var commandInstance = ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, matchedCommand.CommandType);
                var ctx = new CommandDispatchContext(matchedCommand, _commandLineParser.ParseCommand(args, matchedCommand.Options, matchedCommand.Arguments), commandInstance);
                var dispatchAsync = _dispatcherPipelineBuilder.Build();
                return dispatchAsync(ctx);
            }

            throw new CommandNotFoundException(
                string.Empty,
                commandCollection,
                $"No commands are implemented yet."
            );
        }
    }
}
