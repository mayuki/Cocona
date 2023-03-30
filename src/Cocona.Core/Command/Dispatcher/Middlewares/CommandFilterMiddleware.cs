using Cocona.Filters;
using Cocona.Filters.Internal;

namespace Cocona.Command.Dispatcher.Middlewares
{
    public class CommandFilterMiddleware : CommandDispatcherMiddleware
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandFilterMiddleware(CommandDispatchDelegate next, IServiceProvider serviceProvider)
            : base(next)
        {
            _serviceProvider = serviceProvider;
        }

        public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            var filters = FilterHelper.GetFilters<ICommandFilter>(ctx.Command, _serviceProvider);

            CommandExecutionDelegate next = (ctx2) => Next(ctx);

            foreach (var filter in filters)
            {
                var next_ = next;
                next = (ctx2) => filter.OnCommandExecutionAsync(ctx2, next_);
            }

            return next(new CoconaCommandExecutingContext(ctx.Command, ctx.ParsedCommandLine, ctx.CommandTarget));
        }
    }
}
