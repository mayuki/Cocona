using Cocona.Application;
using Cocona.Command;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher;
using Cocona.Command.Dispatcher.Middlewares;
using Cocona.Internal;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Resources;
using Microsoft.Extensions.Options;

namespace Cocona.Hosting
{
    public class CoconaHostedService : IHostedService
    {
        private readonly ICoconaConsoleProvider _console;
        private readonly ICoconaCommandDispatcher _commandDispatcher;
        private readonly ICoconaCommandDispatcherPipelineBuilder _dispatcherPipelineBuilder;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly bool _shouldHandleException;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task? _runningCommandTask;
        private ExceptionDispatchInfo? _capturedException;

        public CoconaHostedService(
            ICoconaConsoleProvider console,
            ICoconaCommandDispatcher commandDispatcher,
            ICoconaCommandDispatcherPipelineBuilder dispatcherPipelineBuilder,
            IHostApplicationLifetime lifetime,
            IOptions<CoconaAppOptions> options)
        {
            _console = console;
            _commandDispatcher = commandDispatcher;
            _dispatcherPipelineBuilder = dispatcherPipelineBuilder;
            _lifetime = lifetime;
            _shouldHandleException = options.Value.HandleExceptionAtRuntime;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dispatcherPipelineBuilder
                .UseMiddleware<HandleExceptionAndExitMiddleware>()
                .UseMiddleware<HandleParameterBindExceptionMiddleware>()
                .UseMiddleware<RejectUnknownOptionsMiddleware>()
                .UseMiddleware<CommandFilterMiddleware>()
                .UseMiddleware<InitializeConsoleAppMiddleware>()
                .UseMiddleware<CoconaCommandInvokeMiddleware>();

            var waitForApplicationStartedTsc = new TaskCompletionSource<bool>();

            _lifetime.ApplicationStarted.Register(() => waitForApplicationStartedTsc.TrySetResult(true));

            _runningCommandTask = ExecuteCoconaApplicationAsync(waitForApplicationStartedTsc.Task);

            return Task.CompletedTask;
        }

        private async Task ExecuteCoconaApplicationAsync(Task waitForApplicationStarted)
        {
            await waitForApplicationStarted.ConfigureAwait(false); // Wait for IHostApplicationLifetime.ApplicationStarted

            try
            {
                Environment.ExitCode = await Task.Run(async () => await _commandDispatcher.DispatchAsync(_cancellationTokenSource.Token));
            }
            catch (CommandNotFoundException cmdNotFoundEx)
            {
                if (string.IsNullOrWhiteSpace(cmdNotFoundEx.Command))
                {
                    _console.Error.WriteLine(string.Format(Strings.Host_Error_CommandNotFound, cmdNotFoundEx.Message));
                }
                else
                {
                    _console.Error.WriteLine(string.Format(Strings.Host_Error_NotACommand, cmdNotFoundEx.Command));
                }

                var similarCommands = cmdNotFoundEx.ImplementedCommands.All.Where(x => Levenshtein.GetDistance(cmdNotFoundEx.Command.ToLowerInvariant(), x.Name.ToLowerInvariant()) < 3).ToArray();
                if (similarCommands.Any())
                {
                    _console.Error.WriteLine();
                    _console.Error.WriteLine(Strings.Host_Error_SimilarCommands);
                    foreach (var c in similarCommands)
                    {
                        _console.Error.WriteLine($"  {c.Name}");
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
                _console.Error.WriteLine($"Unhandled Exception: {ex.GetType().FullName}: {ex.Message}");
                _console.Error.WriteLine(ex.StackTrace);

                Environment.ExitCode = 1;

                _capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            _lifetime.StopApplication();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();

            if (!_shouldHandleException)
            {
                _capturedException?.Throw();
            }

            if (_runningCommandTask != null && !_runningCommandTask.IsCompleted)
            {
                await _runningCommandTask;
            }
        }
    }
}
