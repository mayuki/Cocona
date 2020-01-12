using Cocona;
using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Dispatcher;
using Cocona.CommandLine;
using Cocona.Help;
using Cocona.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoconaSampleApp.Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        [Command(Description = "Greeting from ConsoleApp")]
        public void Echo([Option('n', Description = "Echo message to console.")]string name)
        {
            Console.WriteLine($"Hello from {name}");
        }

        public void Echo2([Argument]string name, [FromService]ILogger<Program> logger)
        {
            Console.WriteLine($"Hello from {name}");
        }

        public void ThrowAndExit()
        {
            throw new CommandExitedException(exitCode: 1);
        }

        [Command(Description = "Long running task demo.")]
        public async Task LongRunning([FromService]ICoconaAppContextAccessor appContext)
        {
            Console.WriteLine("Long running task...");
            
            while (!appContext.Current.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            Console.WriteLine("Canceled.");
        }
    }
}
