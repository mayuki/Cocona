using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Benchmark.External.Commands
{
    public class SystemCommandLineCommand
    {
        public static int ExecuteHandler(string s, int i, bool b) => 0;

        public Task<int> ExecuteAsync(string[] args)
        {
            var command = new RootCommand
            {
                new Option(new[] {"--str", "-s"})
                {
                    Argument = new Argument<string?>()
                },
                new Option(new[] {"--int", "-i"})
                {
                    Argument = new Argument<int>()
                },
                new Option(new[] {"--bool", "-b"})
                {
                    Argument = new Argument<bool>()
                }
            };

            command.Handler = CommandHandler.Create(typeof(SystemCommandLineCommand).GetMethod(nameof(ExecuteHandler)));

            return command.InvokeAsync(args);
        }
    }
}
