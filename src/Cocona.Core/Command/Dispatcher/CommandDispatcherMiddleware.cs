namespace Cocona.Command.Dispatcher
{
    public abstract class CommandDispatcherMiddleware
    {
        protected CommandDispatchDelegate Next { get; }

        protected CommandDispatcherMiddleware(CommandDispatchDelegate next)
        {
            Next = next;
        }

        public abstract ValueTask<int> DispatchAsync(CommandDispatchContext ctx);
    }
}
