using Cocona;
using Cocona.Application;
using Cocona.Command;
using Cocona.Help;
using Cocona.Help.DocumentModel;
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
        [TransformHelp(typeof(LongRunningHelpTransformer))]
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

    class LongRunningHelpTransformer : ICoconaHelpTransformer
    {
        public void TransformHelp(HelpMessage helpMessage, CommandDescriptor command)
        {
            helpMessage.Children.Add(new HelpSection(
                new HelpHeading("Example:"),
                new HelpSection(
                    new HelpParagraph("MyApp --foo --bar")
                )
            ));
        }
    }
}
