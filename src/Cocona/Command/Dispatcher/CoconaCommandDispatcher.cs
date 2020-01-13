using Cocona.Application;
using Cocona.Command.BuiltIn;
using Cocona.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading;
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
        private readonly ICoconaAppContextAccessor _contextAccessor;
        private readonly ILoggerFactory _loggerFactory;

        public CoconaCommandDispatcher(
            IServiceProvider serviceProvider,
            ICoconaCommandProvider commandProvider,
            ICoconaCommandLineParser commandLineParser,
            ICoconaCommandLineArgumentProvider commandLineArgumentProvider,
            ICoconaCommandDispatcherPipelineBuilder dispatcherPipelineBuilder,
            ICoconaCommandMatcher commandMatcher,
            ICoconaAppContextAccessor contextAccessor,
            ILoggerFactory loggerFactory
        )
        {
            _serviceProvider = serviceProvider;
            _commandProvider = commandProvider;
            _commandLineParser = commandLineParser;
            _commandLineArgumentProvider = commandLineArgumentProvider;
            _dispatcherPipelineBuilder = dispatcherPipelineBuilder;
            _commandMatcher = commandMatcher;
            _contextAccessor = contextAccessor;
            _loggerFactory = loggerFactory;
        }

        public async ValueTask<int> DispatchAsync(CancellationToken cancellationToken)
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
                    matchedCommand = commandCollection.Primary ?? throw new CommandNotFoundException("", commandCollection, "A primary command was not found.");
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
                    var preParsedCommandLine = _commandLineParser.ParseCommand(args, matchedCommand.Options, matchedCommand.Arguments);
                    matchedCommand = _commandMatcher.ResolveOverload(matchedCommand, preParsedCommandLine);
                }

                var parsedCommandLine = _commandLineParser.ParseCommand(args, matchedCommand.Options, matchedCommand.Arguments);
                var dispatchAsync = _dispatcherPipelineBuilder.Build();

                _contextAccessor.Current = new CoconaAppContext(
                    cancellationToken,
                    _loggerFactory.CreateLogger(matchedCommand.CommandType)
                );

                // Dispatch command.
                var commandInstance = ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, matchedCommand.CommandType);
                try
                {
                    var ctx = new CommandDispatchContext(matchedCommand, parsedCommandLine, commandInstance);
                    return await dispatchAsync(ctx);
                }
                finally
                {
                    if (commandInstance is IAsyncDisposable asyncDisposable)
                    {
                        await asyncDisposable.DisposeAsync();
                    }
                    else if (commandInstance is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            throw new CommandNotFoundException(
                string.Empty,
                commandCollection,
                $"No commands are implemented yet."
            );
        }
    }
}
