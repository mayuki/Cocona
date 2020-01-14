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
                return await Next(ctx);
            }
            catch (CommandExitedException exitEx)
            {
                return exitEx.ExitCode;
            }
        }
    }
}
