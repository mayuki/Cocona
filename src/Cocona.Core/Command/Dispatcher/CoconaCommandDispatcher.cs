using Cocona.Application;
using Cocona.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Command.Features;
using Cocona.Resources;

namespace Cocona.Command.Dispatcher
{
    public class CoconaCommandDispatcher : ICoconaCommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICoconaCommandDispatcherPipelineBuilder _dispatcherPipelineBuilder;
        private readonly ICoconaInstanceActivator _activator;
        private readonly ICoconaAppContextAccessor _appContext;

        public CoconaCommandDispatcher(
            IServiceProvider serviceProvider,
            ICoconaCommandDispatcherPipelineBuilder dispatcherPipelineBuilder,
            ICoconaInstanceActivator activator,
            ICoconaAppContextAccessor appContext
        )
        {
            _serviceProvider = serviceProvider;
            _dispatcherPipelineBuilder = dispatcherPipelineBuilder;
            _activator = activator;
            _appContext = appContext;
        }

        public async ValueTask<int> DispatchAsync(CommandResolverResult commandResolverResult, CancellationToken cancellationToken)
        {
            if (commandResolverResult.Success)
            {
                // Found a command and dispatch.
                var parsedCommandLine = commandResolverResult.ParsedCommandLine!;
                var matchedCommand = commandResolverResult.MatchedCommand!;
                var subCommandStack = commandResolverResult.SubCommandStack!;

                var dispatchAsync = _dispatcherPipelineBuilder.Build();

                // Activate a command type.
                var commandInstance = default(object);
                var shouldCleanup = false;
                if (matchedCommand.Target is not null)
                {
                    commandInstance = matchedCommand.Target;
                }
                else if (matchedCommand.CommandType.GetConstructors().Any() && !matchedCommand.Method.IsStatic)
                {
                    shouldCleanup = true;
                    commandInstance = _activator.GetServiceOrCreateInstance(_serviceProvider, matchedCommand.CommandType);
                    if (commandInstance == null) throw new InvalidOperationException($"Unable to activate command type '{matchedCommand.CommandType.FullName}'");
                }

                // Set CoconaAppContext
                _appContext.Current = new CoconaAppContext(matchedCommand, cancellationToken);
                _appContext.Current.Features.Set<ICoconaCommandFeature>(new CoconaCommandFeature(commandResolverResult.CommandCollection, matchedCommand, subCommandStack, commandInstance));

                // Dispatch the command
                try
                {
                    var ctx = new CommandDispatchContext(matchedCommand, parsedCommandLine, commandInstance, cancellationToken);
                    return await dispatchAsync(ctx).ConfigureAwait(false);
                }
                finally
                {
                    if (shouldCleanup)
                    {
                        switch (commandInstance)
                        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1
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
            }
            else
            {
                throw new CommandNotFoundException(
                    string.Empty,
                    commandResolverResult.CommandCollection,
                    Strings.Command_NotYetImplemented
                );
            }
        }
    }
}
