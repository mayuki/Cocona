using Cocona.Command;

namespace Cocona.Lite.Hosting
{
    public class CoconaLiteAppHost
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEventSlim _waitForShutdown = new ManualResetEventSlim(false);
        private readonly TimeSpan _shutdownTimeout;

        public IServiceProvider Services => _serviceProvider;

        public CoconaLiteAppHost(IServiceProvider serviceProvider, CoconaLiteAppOptions options)
        {
            _serviceProvider = serviceProvider;
            _shutdownTimeout = options.ShutdownTimeout;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
            var bootstrapper = _serviceProvider.GetRequiredService<ICoconaBootstrapper>();

#pragma warning disable RS0030 // Do not used banned APIs
            Console.CancelKeyPress += OnCancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
#pragma warning restore RS0030 // Do not used banned APIs

            bootstrapper.Initialize();

            try
            {
                var cancellationTask = CreateTaskFromCancellationToken(linkedCancellationToken.Token);
                var runTask = Task.Run(() => bootstrapper.RunAsync(linkedCancellationToken.Token).AsTask());

                var winTask = await Task.WhenAny(cancellationTask, runTask);
                if (winTask != runTask)
                {
                    // Wait for shutdown timeout.
                    var timeoutToken = new CancellationTokenSource(_shutdownTimeout);
                    var winTask2 = await Task.WhenAny(runTask, CreateTaskFromCancellationToken(timeoutToken.Token));
                    if (winTask2 != runTask)
                    {
                        // Timed out. (throw OperationCanceledException)
                        await cancellationTask;
                    }
                }

                Environment.ExitCode = await runTask;
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == linkedCancellationToken.Token)
            {
                // NOTE: Ignore OperationCanceledException that was thrown by non-user code.
                Environment.ExitCode = 130;
            }

            _waitForShutdown.Set();

#pragma warning disable RS0030 // Do not used banned APIs
            Console.CancelKeyPress -= OnCancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
#pragma warning restore RS0030 // Do not used banned APIs

            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void OnProcessExit(object? sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _waitForShutdown.Wait();
        }

        private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _cancellationTokenSource.Cancel();
        }

        private static Task CreateTaskFromCancellationToken(CancellationToken cancellationToken)
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
