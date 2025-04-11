namespace WildCoco.Command.Dispatcher;

public interface IWildCocoCommandDispatcherPipelineBuilder
{
    IWildCocoCommandDispatcherPipelineBuilder UseMiddleware<T>()
        where T: CommandDispatcherMiddleware;
    IWildCocoCommandDispatcherPipelineBuilder UseMiddleware(Func<CommandDispatchDelegate, CommandDispatchContext, ValueTask<int>> commandDispatchDelegate);
    IWildCocoCommandDispatcherPipelineBuilder UseMiddleware(Func<CommandDispatchDelegate, IServiceProvider, CommandDispatcherMiddleware> factory);

    CommandDispatchDelegate Build();
}
