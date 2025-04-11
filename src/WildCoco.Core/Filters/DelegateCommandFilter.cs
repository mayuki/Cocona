using WildCoco.Internal;

namespace WildCoco.Filters;

public class DelegateCommandFilter : ICommandFilter
{
    private readonly Func<WildCocoCommandExecutingContext, CommandExecutionDelegate, ValueTask<int>> _func;

    public DelegateCommandFilter(Func<WildCocoCommandExecutingContext, CommandExecutionDelegate, ValueTask<int>> func)
    {
        ThrowHelper.ThrowIfNull(func);
        _func = func;
    }

    public ValueTask<int> OnCommandExecutionAsync(WildCocoCommandExecutingContext ctx, CommandExecutionDelegate next)
        => _func(ctx, next);
}
