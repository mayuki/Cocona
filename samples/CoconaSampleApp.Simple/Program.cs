using Cocona;
using Cocona.Application;
using Cocona.Command;
using Cocona.Help;
using Cocona.Help.DocumentModel;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
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
        public void Echo([Option('n', Description = "Echo message to console.", ValueName = "Name")]string name)
        {
            Console.WriteLine($"Hello from {name}");
        }

        public void Echo2([Option]string name1, [FromService]ILogger<Program> logger)
        {
            Console.WriteLine($"Hello from {name1}!!!!");
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
                //appContext.Current.CancellationToken.ThrowIfCancellationRequested();
            }

            //var cts = new System.Threading.CancellationTokenSource();
            //cts.Cancel();
            //cts.Token.ThrowIfCancellationRequested();

            Console.WriteLine("Canceled.");
        }

        public void LongRunningSync([FromService]ICoconaAppContextAccessor appContext)
        {
            Console.WriteLine("Long running task...");
            while (!appContext.Current.CancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("Canceled.");
        }

        public void Number(int foo)
        {
            Console.WriteLine(foo);
        }

        public void Enum(User user)
        {
            Console.WriteLine(user);
        }
    }

    public enum User
    {
        Alice, Karen, Other
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
