using Cocona.Application;
using Cocona.Command.Dispatcher;

namespace Cocona.Lite;

public class InitializeCoconaLiteConsoleAppMiddleware : CommandDispatcherMiddleware
{
    private readonly ICoconaAppContextAccessor _appContext;

    public InitializeCoconaLiteConsoleAppMiddleware(CommandDispatchDelegate next, ICoconaAppContextAccessor appContext) : base(next)
    {
        _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
    }

    public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
    {
        if (_appContext.Current != null)
        {
            if (ctx.CommandTarget is CoconaLiteConsoleAppBase consoleApp)
            {
                consoleApp.Context = new CoconaLiteConsoleAppContext(_appContext.Current);
            }
        }

        return Next(ctx);
    }
}
