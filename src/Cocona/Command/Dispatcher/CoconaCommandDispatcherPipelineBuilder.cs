using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cocona.Application;
using Cocona.Command.Binder;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher.Middlewares;
using Cocona.Help;

namespace Cocona.Command.Dispatcher
{
    public delegate ValueTask<int> CommandDispatchDelegate(CommandDispatchContext ctx);

    public class CoconaCommandDispatcherPipelineBuilder : ICoconaCommandDispatcherPipelineBuilder
    {
        private readonly List<(Type? Type, object? Instance)> _typesOrInstances = new List<(Type? Type, object? Instance)>();
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

            for (var i = _typesOrInstances.Count - 1; i >= 0; i--)
            {
                var typeOrInstance = _typesOrInstances[i];
                if (typeOrInstance.Instance is Func<CommandDispatchDelegate, CommandDispatchContext, ValueTask<int>> func)
                {
                    var next_ = next;
                    next = (ctx) => func(next_, ctx);
                }
                else
                {
                    // Optimization: Directly initialize well-known middlewares.
                    if (typeOrInstance.Type == typeof(CoconaCommandInvokeMiddleware))
                    {
                        var m = new CoconaCommandInvokeMiddleware(next, _serviceProvider.GetRequiredService<ICoconaParameterBinder>());
                        next = m.DispatchAsync;
                        continue;
                    }
                    else if (typeOrInstance.Type == typeof(CommandFilterMiddleware))
                    {
                        var m = new CommandFilterMiddleware(next, _serviceProvider);
                        next = m.DispatchAsync;
                        continue;
                    }
                    else if (typeOrInstance.Type == typeof(HandleExceptionAndExitMiddleware))
                    {
                        var m = new HandleExceptionAndExitMiddleware(next, _serviceProvider.GetRequiredService<ICoconaConsoleProvider>());
                        next = m.DispatchAsync;
                        continue;
                    }
                    else if (typeOrInstance.Type == typeof(HandleParameterBindExceptionMiddleware))
                    {
                        var m = new HandleParameterBindExceptionMiddleware(next, _serviceProvider.GetRequiredService<ICoconaConsoleProvider>());
                        next = m.DispatchAsync;
                        continue;
                    }
                    else if (typeOrInstance.Type == typeof(InitializeConsoleAppMiddleware))
                    {
                        var m = new InitializeConsoleAppMiddleware(next, _serviceProvider.GetRequiredService<ICoconaAppContextAccessor>());
                        next = m.DispatchAsync;
                        continue;
                    }
                    else if (typeOrInstance.Type == typeof(RejectUnknownOptionsMiddleware))
                    {
                        var m = new RejectUnknownOptionsMiddleware(next, _serviceProvider.GetRequiredService<ICoconaConsoleProvider>());
                        next = m.DispatchAsync;
                        continue;
                    }
                    else if (typeOrInstance.Type == typeof(BuiltInCommandMiddleware))
                    {
                        var m = new BuiltInCommandMiddleware(
                            next,
                            _serviceProvider.GetRequiredService<ICoconaHelpRenderer>(),
                            _serviceProvider.GetRequiredService<ICoconaCommandHelpProvider>(),
                            _serviceProvider.GetRequiredService<ICoconaCommandProvider>(),
                            _serviceProvider.GetRequiredService<ICoconaConsoleProvider>());
                        next = m.DispatchAsync;
                        continue;
                    }

                    var middleware = (CommandDispatcherMiddleware)ActivatorUtilities.CreateInstance(_serviceProvider, typeOrInstance.Type, next);
                    next = middleware.DispatchAsync;
                }
            }

            return next;
        }
    }
}
