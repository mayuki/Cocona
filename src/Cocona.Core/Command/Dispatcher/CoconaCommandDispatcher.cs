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
        private readonly ICoconaCommandLineArgumentProvider _commandLineArgumentProvider;
        private readonly ICoconaCommandResolver _commandResolver;
        private readonly ICoconaCommandDispatcherPipelineBuilder _dispatcherPipelineBuilder;
        private readonly ICoconaInstanceActivator _activator;
        private readonly ICoconaAppContextAccessor _appContext;

        public CoconaCommandDispatcher(
            IServiceProvider serviceProvider,
            ICoconaCommandLineArgumentProvider commandLineArgumentProvider,
            ICoconaCommandResolver commandResolver,
            ICoconaCommandDispatcherPipelineBuilder dispatcherPipelineBuilder,
            ICoconaInstanceActivator activator,
            ICoconaAppContextAccessor appContext
        )
        {
            _serviceProvider = serviceProvider;
            _commandLineArgumentProvider = commandLineArgumentProvider;
            _commandResolver = commandResolver;
            _dispatcherPipelineBuilder = dispatcherPipelineBuilder;
            _activator = activator;
            _appContext = appContext;
        }

        public async ValueTask<int> DispatchAsync(CancellationToken cancellationToken)
        {
            var result = _commandResolver.ParseAndResolve(_commandLineArgumentProvider.GetArguments());
            if (result.Success)
            {
                // Found a command and dispatch.
                var parsedCommandLine = result.ParsedCommandLine!;
                var matchedCommand = result.MatchedCommand!;
                var subCommandStack = result.SubCommandStack!;

                var dispatchAsync = _dispatcherPipelineBuilder.Build();

                // Activate a command type.
                var commandInstance = default(object);
                if (matchedCommand.Target is not null)
                {
                    commandInstance = matchedCommand.Target;
                }
                else if (matchedCommand.CommandType.GetConstructors().Any() && !matchedCommand.Method.IsStatic)
                {
                    commandInstance = _activator.GetServiceOrCreateInstance(_serviceProvider, matchedCommand.CommandType);
                    if (commandInstance == null) throw new InvalidOperationException($"Unable to activate command type '{matchedCommand.CommandType.FullName}'");
                }

                // Set CoconaAppContext
                _appContext.Current = new CoconaAppContext(matchedCommand, cancellationToken);
                _appContext.Current.Features.Set<ICoconaCommandFeature>(new CoconaCommandFeature(result.CommandCollection, matchedCommand, subCommandStack, commandInstance));

                // Dispatch the command
                try
                {
                    var ctx = new CommandDispatchContext(matchedCommand, parsedCommandLine, commandInstance, cancellationToken);
                    return await dispatchAsync(ctx).ConfigureAwait(false);
                }
                finally
                {
                    switch (commandInstance)
                    {
#if NET5_0 || NETSTANDARD2_1
                        case IAsyncDisposable asyncDisposable:
                            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                            break;
#endif
                        case IDisposable disposable:
                            disposable.Dispose();
                            break;
                    }
                }
            }
            else
            {
                throw new CommandNotFoundException(
                    string.Empty,
                    result.CommandCollection,
                    $"Command not yet implemented."
                );
            }
        }
    }
}
