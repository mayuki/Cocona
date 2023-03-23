using System;
using System.Threading.Tasks;
using Cocona.Internal;

namespace Cocona.Filters
{
    public class DelegateCommandFilter : ICommandFilter
    {
        private readonly Func<CoconaCommandExecutingContext, CommandExecutionDelegate, ValueTask<int>> _func;

        public DelegateCommandFilter(Func<CoconaCommandExecutingContext, CommandExecutionDelegate, ValueTask<int>> func)
        {
            ThrowHelper.ThrowIfNull(func);
            _func = func;
        }

        public ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
            => _func(ctx, next);
    }
}
