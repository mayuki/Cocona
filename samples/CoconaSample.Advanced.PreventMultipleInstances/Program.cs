using System;
using System.Threading;
using System.Threading.Tasks;
using Cocona;

namespace CoconaSample.Advanced.PreventMultipleInstances
{
    [PreventMultipleInstances]
    class Program : CoconaConsoleAppBase
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Start long-running task...");

            while (!Context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }
            await Task.Run(() => Task.Delay(100));

            Console.WriteLine("Canceled");
        }
    }
}
