using Cocona.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher.Middlewares
{
    public class InitializeConsoleAppMiddleware : CommandDispatcherMiddleware
    {
        private readonly ICoconaAppContextAccessor _appContext;

        public InitializeConsoleAppMiddleware(CommandDispatchDelegate next, ICoconaAppContextAccessor appContext) : base(next)
        {
            _appContext = appContext;
        }

        public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            if (ctx.CommandTarget is CoconaConsoleAppBase consoleApp)
            {
                consoleApp.Context = _appContext.Current!;
            }

            return Next(ctx);
        }
    }
}
