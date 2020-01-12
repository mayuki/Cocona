using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher
{
    public delegate ValueTask<int> CommandDispatchDelegate(CommandDispatchContext ctx);

    public class CoconaCommandDispatcherPipelineBuilder : ICoconaCommandDispatcherPipelineBuilder
    {
        private readonly List<(Type? Type, Func<CommandDispatchDelegate, CommandDispatchContext, ValueTask<int>>? Instance)> _typesOrInstances = new List<(Type? Type, Func<CommandDispatchDelegate, CommandDispatchContext, ValueTask<int>>? Instance)>();
        private readonly IServiceProvider _serviceProvider;

        public CoconaCommandDispatcherPipelineBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICoconaCommandDispatcherPipelineBuilder UseMiddleware<T>()
            where T: CommandDispatcherMiddleware
        {
            var t = typeof(T);

            if (t.IsAbstract || (t.IsGenericType && !t.IsConstructedGenericType))
                throw new ArgumentException("A middleware type must be constructed class.");

            _typesOrInstances.Add((t, null));
            return this;
        }

        public ICoconaCommandDispatcherPipelineBuilder UseMiddleware(Func<CommandDispatchDelegate, CommandDispatchContext, ValueTask<int>> middleware)
        {
            _typesOrInstances.Add((null, middleware));
            return this;
        }

        public CommandDispatchDelegate Build()
        {
            CommandDispatchDelegate next = (ctx) => new ValueTask<int>(0);

            foreach (var typeOrInstance in _typesOrInstances.AsEnumerable().Reverse())
            {
                if (typeOrInstance.Instance != null)
                {
                    var next_ = next;
                    next = (ctx) => typeOrInstance.Instance(next_, ctx);
                }
                else
                {
                    var middleware = (CommandDispatcherMiddleware)ActivatorUtilities.CreateInstance(_serviceProvider, typeOrInstance.Type, next);
                    next = middleware.DispatchAsync;
                }
            }

            return next;
        }
    }
}
