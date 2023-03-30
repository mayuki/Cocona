namespace Cocona.Command.Dispatcher
{
    public interface ICoconaCommandDispatcher
    {
        ValueTask<int> DispatchAsync(CommandResolverResult commandResolverResult, CancellationToken cancellationToken = default);
    }
}
