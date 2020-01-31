using System;
using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher
{
    public interface ICoconaCommandDispatcherPipelineBuilder
    {
        ICoconaCommandDispatcherPipelineBuilder UseMiddleware<T>()
            where T: CommandDispatcherMiddleware;
        ICoconaCommandDispatcherPipelineBuilder UseMiddleware(Func<CommandDispatchDelegate, CommandDispatchContext, ValueTask<int>> commandDispatchDelegate);

        CommandDispatchDelegate Build();
    }
}
