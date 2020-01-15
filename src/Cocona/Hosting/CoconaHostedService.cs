using Cocona.Command;
using Cocona.Command.Dispatcher;
using Cocona.Command.Dispatcher.Middlewares;
using Cocona.Internal;
using Microsoft.Extensions.Hosting;
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
        private readonly IHostApplicationLifetime _lifetime;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task? _runningCommandTask;

        public CoconaHostedService(
            ICoconaCommandDispatcher commandDispatcher,
            ICoconaCommandDispatcherPipelineBuilder dispatcherPipelineBuilder,
            IHostApplicationLifetime lifetime
        )
        {
            _commandDispatcher = commandDispatcher;
            _dispatcherPipelineBuilder = dispatcherPipelineBuilder;
            _lifetime = lifetime;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dispatcherPipelineBuilder
                .UseMiddleware<CommandHelpMiddleware>()
                .UseMiddleware<HandleExceptionAndExitMiddleware>()
                .UseMiddleware<HandleParameterBindExceptionMiddleware>()
                .UseMiddleware<RejectUnknownOptionsMiddleware>()
                .UseMiddleware<InitializeConsoleAppMiddleware>()
                .UseMiddleware<CoconaCommandInvokeMiddleware>();

            _lifetime.ApplicationStarted.Register(async () =>
            {
                async Task RunAsync()
                {
                    try
                    {
                        Environment.ExitCode = await Task.Run(async () => await _commandDispatcher.DispatchAsync(_cancellationTokenSource.Token));
                    }
                    catch (CommandNotFoundException cmdNotFoundEx)
                    {
                        if (string.IsNullOrWhiteSpace(cmdNotFoundEx.Command))
                        {
                            Console.Error.WriteLine($"Error: {cmdNotFoundEx.Message}");
                        }
                        else
                        {
                            Console.Error.WriteLine($"Error: '{cmdNotFoundEx.Command}' is not a command. See '--help'");
                        }

                        var similarCommands = cmdNotFoundEx.ImplementedCommands.All.Where(x => Levenshtein.GetDistance(cmdNotFoundEx.Command.ToLowerInvariant(), x.Name.ToLowerInvariant()) < 3).ToArray();
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
                    catch (OperationCanceledException ex) when (ex.CancellationToken == _cancellationTokenSource.Token)
                    {
                        // NOTE: Ignore OperationCanceledException that was thrown by non-user code.
                        Environment.ExitCode = 0;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Unhandled Exception: {ex.GetType().FullName}: {ex.Message}");
                        Console.Error.WriteLine(ex.StackTrace);
                        Environment.ExitCode = 1;
                    }
                }

                try
                {
                    _runningCommandTask = RunAsync();
                    await _runningCommandTask;
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
