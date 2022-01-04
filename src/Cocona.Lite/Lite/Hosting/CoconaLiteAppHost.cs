using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Dispatcher;
using Cocona.Internal;

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
            var commandDispatcher = _serviceProvider.GetRequiredService<ICoconaCommandDispatcher>();
            var console = _serviceProvider.GetRequiredService<ICoconaConsoleProvider>();
            var shouldHandleException = _serviceProvider.GetRequiredService<CoconaLiteAppOptions>().HandleExceptionAtRuntime;

#pragma warning disable RS0030 // Do not used banned APIs
            Console.CancelKeyPress += OnCancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
#pragma warning restore RS0030 // Do not used banned APIs

            try
            {
                var task = commandDispatcher.DispatchAsync(linkedCancellationToken.Token);
                if (task.IsCompleted)
                {
                    Environment.ExitCode = task.Result;
                }
                else
                {
                    Environment.ExitCode = await task;
                }
            }
            catch (CommandNotFoundException cmdNotFoundEx)
            {
                if (string.IsNullOrWhiteSpace(cmdNotFoundEx.Command))
                {
                    console.Error.WriteLine($"Error: {cmdNotFoundEx.Message}");
                }
                else
                {
                    console.Error.WriteLine($"Error: '{cmdNotFoundEx.Command}' is not a command. See '--help' for usage.");
                }

                var similarCommands = cmdNotFoundEx.ImplementedCommands.All.Where(x => Levenshtein.GetDistance(cmdNotFoundEx.Command.ToLowerInvariant(), x.Name.ToLowerInvariant()) < 3).ToArray();
                if (similarCommands.Any())
                {
                    console.Error.WriteLine();
                    console.Error.WriteLine("Similar commands:");
                    foreach (var c in similarCommands)
                    {
                        console.Error.WriteLine($"  {c.Name}");
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
                console.Error.WriteLine($"Unhandled Exception: {ex.GetType().FullName}: {ex.Message}");
                console.Error.WriteLine(ex.StackTrace);

                Environment.ExitCode = 1;

                if (!shouldHandleException)
                {
                    throw new AggregateException(ex); // NOTE: Align behavior with non-Lite versions.
                }
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
