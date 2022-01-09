using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Dispatcher;
using Cocona.Internal;
using Cocona.Lite.Resources;

namespace Cocona.Lite.Hosting
{
    public class CoconaLiteAppHost
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEventSlim _waitForShutdown = new ManualResetEventSlim(false);

        public IServiceProvider Services => _serviceProvider;

        public CoconaLiteAppHost(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
                var task = bootstrapper.RunAsync(linkedCancellationToken.Token);
                if (task.IsCompleted)
                {
                    Environment.ExitCode = task.Result;
                }
                else
                {
                    Environment.ExitCode = await task;
                }
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == _cancellationTokenSource.Token)
            {
                // NOTE: Ignore OperationCanceledException that was thrown by non-user code.
                Environment.ExitCode = 0;
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
    }
}
