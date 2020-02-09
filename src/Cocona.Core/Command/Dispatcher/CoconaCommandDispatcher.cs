using Cocona.Application;
using Cocona.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Command.Features;

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
        private readonly ICoconaInstanceActivator _activator;
        private readonly ICoconaAppContextAccessor _appContext;

        public CoconaCommandDispatcher(
            IServiceProvider serviceProvider,
            ICoconaCommandProvider commandProvider,
            ICoconaCommandLineParser commandLineParser,
            ICoconaCommandLineArgumentProvider commandLineArgumentProvider,
            ICoconaCommandDispatcherPipelineBuilder dispatcherPipelineBuilder,
            ICoconaCommandMatcher commandMatcher,
            ICoconaInstanceActivator activator,
            ICoconaAppContextAccessor appContext
        )
        {
            _serviceProvider = serviceProvider;
            _commandProvider = commandProvider;
            _commandLineParser = commandLineParser;
            _commandLineArgumentProvider = commandLineArgumentProvider;
            _dispatcherPipelineBuilder = dispatcherPipelineBuilder;
            _commandMatcher = commandMatcher;
            _activator = activator;
            _appContext = appContext;
        }

        public async ValueTask<int> DispatchAsync(CancellationToken cancellationToken)
        {
            var commandCollection = _commandProvider.GetCommandCollection();
            var args = _commandLineArgumentProvider.GetArguments();
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

                    // If the command have nested sub-commands, try to restart parse command.
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
                    var preParsedCommandLine = _commandLineParser.ParseCommand(args, matchedCommand.Options, matchedCommand.Arguments);
                    matchedCommand = _commandMatcher.ResolveOverload(matchedCommand, preParsedCommandLine);
                }

                var parsedCommandLine = _commandLineParser.ParseCommand(args, matchedCommand.Options, matchedCommand.Arguments);
                var dispatchAsync = _dispatcherPipelineBuilder.Build();

                // Activate a command type.
                var commandInstance = _activator.GetServiceOrCreateInstance(_serviceProvider, matchedCommand.CommandType);
                if (commandInstance == null) throw new InvalidOperationException($"Unable to activate command type '{matchedCommand.CommandType.FullName}'");

                // Set CoconaAppContext
                _appContext.Current = new CoconaAppContext(matchedCommand, cancellationToken);
                _appContext.Current.Features.Set<ICoconaCommandFeature>(new CoconaCommandFeature(commandCollection, matchedCommand, subCommandStack, commandInstance));

                // Dispatch the command
                try
                {
                    var ctx = new CommandDispatchContext(matchedCommand, parsedCommandLine, commandInstance, cancellationToken);
                    return await dispatchAsync(ctx);
                }
                finally
                {
                    if (commandInstance is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            throw new CommandNotFoundException(
                string.Empty,
                commandCollection,
                $"Command not yet implemented."
            );
        }
    }
}
