using WildCoco.Application;
using WildCoco.Command.Dispatcher;
using WildCoco.Application;
using WildCoco.Command.Dispatcher;

namespace WildCoco.Lite;

public class InitializeWildCocoLiteConsoleAppMiddleware : CommandDispatcherMiddleware
{
    private readonly IWildCocoAppContextAccessor _appContext;

    public InitializeWildCocoLiteConsoleAppMiddleware(CommandDispatchDelegate next, IWildCocoAppContextAccessor appContext) : base(next)
    {
        _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
    }

    public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
    {
        if (_appContext.Current != null)
        {
            if (ctx.CommandTarget is WildCocoLiteConsoleAppBase consoleApp)
            {
                consoleApp.Context = new WildCocoLiteConsoleAppContext(_appContext.Current);
            }
        }

        return Next(ctx);
    }
}
