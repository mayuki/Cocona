using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Dispatcher;
using Cocona.Command.Dispatcher.Middlewares;
using Cocona.CommandLine;
using Cocona.Help;
using Cocona.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cocona.Hosting
{
    public class CoconaHostedService : IHostedService
    {
        private readonly ICoconaCommandDispatcher _commandDispatcher;
        private readonly ICoconaCommandDispatcherPipelineBuilder _dispatcherPipelineBuilder;
        private readonly ICoconaHelpRenderer _helpRenderer;
        private readonly ICoconaCommandHelpProvider _commandHelpProvider;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ICoconaAppContextAccessor _contextAccessor;

        private readonly CoconaAppContext _context;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task<int>? _runningCommandTask;

        public CoconaHostedService(
            ICoconaHelpRenderer helpRenderer,
            ICoconaCommandHelpProvider commandHelpProvider,
            ICoconaCommandDispatcher commandDispatcher,
            ICoconaCommandDispatcherPipelineBuilder dispatcherPipelineBuilder,
            ICoconaAppContextAccessor contextAccessor,
            ILogger<CoconaHostedService> logger,
            IHostApplicationLifetime lifetime
        )
        {
            _helpRenderer = helpRenderer;
            _commandHelpProvider = commandHelpProvider;
            _commandDispatcher = commandDispatcher;
            _dispatcherPipelineBuilder = dispatcherPipelineBuilder;
            _contextAccessor = contextAccessor;
            _lifetime = lifetime;

            _cancellationTokenSource = new CancellationTokenSource();
            _context = new CoconaAppContext(_cancellationTokenSource.Token, logger);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dispatcherPipelineBuilder
                .UseMiddleware<HelpAndVersionMiddleware>()
                .UseMiddleware<HandleExceptionAndExitMiddleware>()
                .UseMiddleware<HandleParameterBindExceptionMiddleware>()
                .UseMiddleware<RejectUnknownOptionsMiddleware>()
                .UseMiddleware<CoconaCommandInvokeMiddleware>();

            _contextAccessor.Current = _context;

            _lifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    _runningCommandTask = _commandDispatcher.DispatchAsync().AsTask();
                    await _runningCommandTask;
                }
                catch (CommandNotFoundException cmdNotFoundEx)
                {
                    Console.Error.WriteLine($"Error: '{cmdNotFoundEx.Command}' is not a command. See '--help'");

                    var similarCommands = cmdNotFoundEx.ImplementedCommands.All.Where(x => Levenshtein.GetDistance(cmdNotFoundEx.Command, x.Name) < 3).ToArray();
                    if (similarCommands.Any())
                    {
                        Console.Error.WriteLine();
                        Console.Error.WriteLine("Similar commands:");
                        foreach (var c in similarCommands)
                        {
                            Console.Error.WriteLine($"  {c.Name}");
                        }
                    }

                    Environment.ExitCode = 1;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unhandled Exception: {ex.Message}");
                    Console.Error.WriteLine(ex.StackTrace);
                    Environment.ExitCode = 1;
                }
                finally
                {
                    _lifetime.StopApplication();
                }
            });

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();

            if (_runningCommandTask != null && !_runningCommandTask.IsCompleted)
            {
                await _runningCommandTask;
            }
        }
    }
}
