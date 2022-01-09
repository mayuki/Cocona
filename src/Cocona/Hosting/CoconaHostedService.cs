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
        private readonly ICoconaBootstrapper _bootstrapper;
        private readonly ICoconaCommandDispatcherPipelineBuilder _dispatcherPipelineBuilder;
        private readonly IHostApplicationLifetime _lifetime;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task? _runningCommandTask;
        private ExceptionDispatchInfo? _capturedException;

        public CoconaHostedService(
            ICoconaConsoleProvider console,
            ICoconaBootstrapper bootstrapper,
            ICoconaCommandDispatcherPipelineBuilder dispatcherPipelineBuilder,
            IHostApplicationLifetime lifetime,
            IOptions<CoconaAppOptions> options)
        {
            _console = console;
            _bootstrapper = bootstrapper;
            _dispatcherPipelineBuilder = dispatcherPipelineBuilder;
            _lifetime = lifetime;

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

            // Build command collection and parse the command line.
            _bootstrapper.Initialize();

            _runningCommandTask = ExecuteCoconaApplicationAsync(waitForApplicationStartedTsc.Task);

            return Task.CompletedTask;
        }

        private async Task ExecuteCoconaApplicationAsync(Task waitForApplicationStarted)
        {
            await waitForApplicationStarted.ConfigureAwait(false); // Wait for IHostApplicationLifetime.ApplicationStarted

            try
            {
                Environment.ExitCode = await Task.Run(async () => await _bootstrapper.RunAsync(_cancellationTokenSource.Token));
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == _cancellationTokenSource.Token)
            {
                // NOTE: Ignore OperationCanceledException that was thrown by non-user code.
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;
                _capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            _lifetime.StopApplication();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();

            _capturedException?.Throw();

            if (_runningCommandTask != null && !_runningCommandTask.IsCompleted)
            {
                await _runningCommandTask;
            }
        }
    }
}
