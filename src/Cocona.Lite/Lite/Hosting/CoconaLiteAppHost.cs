using System;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Command.Dispatcher;

namespace Cocona.Lite.Hosting
{
    public class CoconaLiteAppHost
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEventSlim _waitForShutdown = new ManualResetEventSlim(false);

        public CoconaLiteAppHost(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task RunAsyncCore(CancellationToken cancellationToken)
        {
            var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
            var commandDispatcher = _serviceProvider.GetService<ICoconaCommandDispatcher>();

#pragma warning disable RS0030 // Do not used banned APIs
            Console.CancelKeyPress += OnCancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
#pragma warning restore RS0030 // Do not used banned APIs

            Environment.ExitCode = await Task.Run(async () => await commandDispatcher.DispatchAsync(linkedCancellationToken.Token));

            _waitForShutdown.Set();
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _waitForShutdown.Wait();
        }

        private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _cancellationTokenSource.Cancel();
        }
    }
}