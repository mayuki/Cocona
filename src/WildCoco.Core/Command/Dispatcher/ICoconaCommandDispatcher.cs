namespace WildCoco.Command.Dispatcher;

public interface IWildCocoCommandDispatcher
{
    ValueTask<int> DispatchAsync(CommandResolverResult commandResolverResult, CancellationToken cancellationToken = default);
}
