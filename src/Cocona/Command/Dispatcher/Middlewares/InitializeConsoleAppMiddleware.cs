using Cocona.Application;
using Microsoft.Extensions.Logging;

namespace Cocona.Command.Dispatcher.Middlewares;

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
        if (_appContext.Current != null)
        {
            _appContext.Current.Features.Set<ILogger>(_loggerFactory.CreateLogger(ctx.Command.CommandType));

            if (ctx.CommandTarget is CoconaConsoleAppBase consoleApp)
            {
                consoleApp.Context = new CoconaConsoleAppContext(_appContext.Current);
            }
        }

        return Next(ctx);
    }
}
