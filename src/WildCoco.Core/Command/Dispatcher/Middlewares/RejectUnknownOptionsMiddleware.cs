using WildCoco.Application;
using WildCoco.Resources;

namespace WildCoco.Command.Dispatcher.Middlewares;

public class RejectUnknownOptionsMiddleware : CommandDispatcherMiddleware
{
    private readonly IWildCocoConsoleProvider _console;

    public RejectUnknownOptionsMiddleware(CommandDispatchDelegate next, IWildCocoConsoleProvider console) : base(next)
    {
        _console = console;
    }

    public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
    {
        if (ctx.ParsedCommandLine.UnknownOptions.Count > 0)
        {
            if (!ctx.Command.Flags.HasFlag(CommandFlags.IgnoreUnknownOptions))
            {
                _console.Error.WriteLine(string.Format(Strings.Command_Error_UnknownOption, ctx.ParsedCommandLine.UnknownOptions[0]));
                return new ValueTask<int>(129);
            }
        }

        return Next(ctx);
    }
}
