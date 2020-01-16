using Cocona.Application;
using Cocona.Command.Binder;
using Cocona.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher.Middlewares
{
    public class HandleExceptionAndExitMiddleware : CommandDispatcherMiddleware
    {
        private readonly ICoconaConsoleProvider _console;

        public HandleExceptionAndExitMiddleware(CommandDispatchDelegate next, ICoconaConsoleProvider console) : base(next)
        {
            _console = console;
        }

        public override async ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            try
            {
                return await Next(ctx);
            }
            catch (CommandExitedException exitEx)
            {
                if (!string.IsNullOrWhiteSpace(exitEx.ExitMessage))
                {
                    // Write the message to stderr if exit code was non-zero.
                    if (exitEx.ExitCode == 0)
                    {
                        _console.Output.WriteLine(exitEx.ExitMessage);
                    }
                    else
                    {
                        _console.Error.WriteLine(exitEx.ExitMessage);
                    }
                }
                return exitEx.ExitCode;
            }
        }
    }
}
