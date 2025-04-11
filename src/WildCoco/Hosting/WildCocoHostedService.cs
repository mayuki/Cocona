using WildCoco.Application;
using WildCoco.Command;
using WildCoco.Command.Dispatcher;
using WildCoco.Command.Dispatcher.Middlewares;
using WildCoco.Application;
using WildCoco.Command;
using WildCoco.Command.Dispatcher;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WildCoco.Command.Dispatcher.Middlewares;

namespace WildCoco.Hosting;

public class WildCocoHostedService : IHostedService
{
    private readonly IWildCocoConsoleProvider _console;
    private readonly IWildCocoBootstrapper _bootstrapper;
    private readonly IWildCocoCommandDispatcherPipelineBuilder _dispatcherPipelineBuilder;
    private readonly IHostApplicationLifetime _lifetime;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task? _runningCommandTask;

    public WildCocoHostedService(
        IWildCocoConsoleProvider console,
        IWildCocoBootstrapper bootstrapper,
        IWildCocoCommandDispatcherPipelineBuilder dispatcherPipelineBuilder,
        IHostApplicationLifetime lifetime,
        IOptions<WildCocoAppOptions> options)
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
            .UseMiddleware<WildCocoCommandInvokeMiddleware>();

        var waitForApplicationStartedTsc = new TaskCompletionSource<bool>();

        _lifetime.ApplicationStarted.Register(() => waitForApplicationStartedTsc.TrySetResult(true));

        // Build command collection and parse the command line.
        _bootstrapper.Initialize();

        _runningCommandTask = ExecuteWildCocoApplicationAsync(waitForApplicationStartedTsc.Task);

        return Task.CompletedTask;
    }

    private async Task ExecuteWildCocoApplicationAsync(Task waitForApplicationStarted)
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
            _console.Error.WriteLine(ex.ToString());
            Environment.ExitCode = 1;
        }
        finally
        {
            _lifetime.StopApplication();
        }
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
