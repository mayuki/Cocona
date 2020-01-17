using System;
using System.Threading.Tasks;
using Cocona;

namespace CoconaSample.InAction.HandleShutdownSignal
{
    class Program : CoconaConsoleAppBase
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Press Ctrl+C to shutdown the application.");
            Console.WriteLine("Start long-running task...");

            while (!Context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            Console.WriteLine("Shutting down...");
        }
    }
}
