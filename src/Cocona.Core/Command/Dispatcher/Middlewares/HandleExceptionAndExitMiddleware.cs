using Cocona.Application;
using System;
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
            catch (Exception ex)
            {
                _console.Error.WriteLine($"Unhandled Exception: {ex.GetType().FullName}: {ex.Message}");
                _console.Error.WriteLine(ex.StackTrace);

                return 1;
            }
        }
    }
}
