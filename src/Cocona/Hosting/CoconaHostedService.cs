using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Dispatcher;
using Cocona.Command.Dispatcher.Middlewares;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Cocona.Hosting;

public class CoconaHostedService : IHostedService
{
    private readonly ICoconaConsoleProvider _console;
    private readonly ICoconaBootstrapper _bootstrapper;
    private readonly ICoconaCommandDispatcherPipelineBuilder _dispatcherPipelineBuilder;
    private readonly IHostApplicationLifetime _lifetime;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task? _runningCommandTask;

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
        catch (Exception)
        {
            Environment.ExitCode = 1;
            throw;
        }

        _lifetime.StopApplication();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();

        if (_runningCommandTask != null && !_runningCommandTask.IsCompleted)
        {
            var cancellationTask = CreateTaskFromCancellationToken(cancellationToken);
            try
            {
                var winTask = await Task.WhenAny(cancellationTask, _runningCommandTask);
                if (winTask == _runningCommandTask)
                {
                    await _runningCommandTask;
                }
            }
            catch (OperationCanceledException e) when (e.CancellationToken == cancellationToken)
            {
                Environment.ExitCode = 130;
            }
        }

        static Task CreateTaskFromCancellationToken(CancellationToken cancellationToken)
        {
            var tsc = new TaskCompletionSource<bool>();
            cancellationToken.Register(() =>
            {
                tsc.TrySetCanceled(cancellationToken);
            });
            return tsc.Task;
        }
    }
}
