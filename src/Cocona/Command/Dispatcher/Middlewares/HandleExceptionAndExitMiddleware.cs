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
        public HandleExceptionAndExitMiddleware(CommandDispatchDelegate next) : base(next)
        {
        }

        public override async ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            try
            {
                var result = await Next(ctx);
                Environment.ExitCode = result;
                return result;
            }
            catch (CommandExitedException exitEx)
            {
                Environment.ExitCode = exitEx.ExitCode;
                return exitEx.ExitCode;
            }
            catch
            {
                Environment.ExitCode = 1;
                throw;
            }
        }
    }
}
