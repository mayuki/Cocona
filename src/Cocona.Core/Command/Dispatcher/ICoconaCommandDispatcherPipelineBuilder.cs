namespace Cocona.Command.Dispatcher;

public interface ICoconaCommandDispatcherPipelineBuilder
{
    ICoconaCommandDispatcherPipelineBuilder UseMiddleware<T>()
        where T: CommandDispatcherMiddleware;
    ICoconaCommandDispatcherPipelineBuilder UseMiddleware(Func<CommandDispatchDelegate, CommandDispatchContext, ValueTask<int>> commandDispatchDelegate);
    ICoconaCommandDispatcherPipelineBuilder UseMiddleware(Func<CommandDispatchDelegate, IServiceProvider, CommandDispatcherMiddleware> factory);

    CommandDispatchDelegate Build();
}