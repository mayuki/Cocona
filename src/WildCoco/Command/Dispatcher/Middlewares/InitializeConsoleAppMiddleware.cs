using WildCoco.Application;
using WildCoco.Command.Dispatcher;
using WildCoco.Application;
using Microsoft.Extensions.Logging;

namespace WildCoco.Command.Dispatcher.Middlewares;

public class InitializeConsoleAppMiddleware : CommandDispatcherMiddleware
{
    private readonly IWildCocoAppContextAccessor _appContext;
    private readonly ILoggerFactory _loggerFactory;

    public InitializeConsoleAppMiddleware(CommandDispatchDelegate next, IWildCocoAppContextAccessor appContext, ILoggerFactory loggerFactory) : base(next)
    {
        _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
    {
        if (_appContext.Current != null)
        {
            _appContext.Current.Features.Set<ILogger>(_loggerFactory.CreateLogger(ctx.Command.CommandType));

            if (ctx.CommandTarget is WildCocoConsoleAppBase consoleApp)
            {
                consoleApp.Context = new WildCocoConsoleAppContext(_appContext.Current);
            }
        }

        return Next(ctx);
    }
}
