using Cocona.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Cocona.Command.Dispatcher.Middlewares
{
    public class InitializeConsoleAppMiddleware : CommandDispatcherMiddleware
    {
        private readonly ICoconaAppContextAccessor _appContext;
        private readonly ILoggerFactory _loggerFactory;

        public InitializeConsoleAppMiddleware(CommandDispatchDelegate next, ICoconaAppContextAccessor appContext, ILoggerFactory loggerFactory) : base(next)
        {
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            _appContext.Current = new CoconaAppContext(
                ctx.CancellationToken,
                _loggerFactory.CreateLogger(ctx.Command.CommandType)
            );

            if (ctx.CommandTarget is CoconaConsoleAppBase consoleApp)
            {
                consoleApp.Context = _appContext.Current!;
            }

            return Next(ctx);
        }
    }
}
