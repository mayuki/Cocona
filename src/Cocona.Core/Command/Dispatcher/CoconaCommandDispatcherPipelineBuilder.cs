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
using Cocona.ShellCompletion;

namespace Cocona.Command.Dispatcher
{
    public delegate ValueTask<int> CommandDispatchDelegate(CommandDispatchContext ctx);

    public class CoconaCommandDispatcherPipelineBuilder : ICoconaCommandDispatcherPipelineBuilder
    {
        private readonly List<(Type? Type, object? Instance)> _typesOrInstances = new List<(Type? Type, object? Instance)>();
        private readonly IServiceProvider _serviceProvider;
        private readonly ICoconaInstanceActivator _activator;

        public CoconaCommandDispatcherPipelineBuilder(IServiceProvider serviceProvider, ICoconaInstanceActivator activator)
        {
            _serviceProvider = serviceProvider;
            _activator = activator;
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

        public ICoconaCommandDispatcherPipelineBuilder UseMiddleware(Func<CommandDispatchDelegate, IServiceProvider, CommandDispatcherMiddleware> factory)
        {
            _typesOrInstances.Add((null, factory));
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
                else if (typeOrInstance.Instance is Func<CommandDispatchDelegate, IServiceProvider, CommandDispatcherMiddleware> factory)
                {
                    next = factory(next, _serviceProvider).DispatchAsync;
                }
                else
                {
                    // Optimization: Directly initialize well-known middlewares.
                    if (typeOrInstance.Type != null)
                    {
                        var t = typeOrInstance.Type;
                        if (t == typeof(CoconaCommandInvokeMiddleware))
                        {
                            var m = new CoconaCommandInvokeMiddleware(next, GetRequiredService<ICoconaParameterBinder>(_serviceProvider));
                            next = m.DispatchAsync;
                            continue;
                        }
                        else if (t == typeof(CommandFilterMiddleware))
                        {
                            var m = new CommandFilterMiddleware(next, _serviceProvider);
                            next = m.DispatchAsync;
                            continue;
                        }
                        else if (t == typeof(HandleExceptionAndExitMiddleware))
                        {
                            var m = new HandleExceptionAndExitMiddleware(next, GetRequiredService<ICoconaConsoleProvider>(_serviceProvider));
                            next = m.DispatchAsync;
                            continue;
                        }
                        else if (t == typeof(HandleParameterBindExceptionMiddleware))
                        {
                            var m = new HandleParameterBindExceptionMiddleware(next, GetRequiredService<ICoconaConsoleProvider>(_serviceProvider));
                            next = m.DispatchAsync;
                            continue;
                        }
                        else if (t == typeof(RejectUnknownOptionsMiddleware))
                        {
                            var m = new RejectUnknownOptionsMiddleware(next, GetRequiredService<ICoconaConsoleProvider>(_serviceProvider));
                            next = m.DispatchAsync;
                            continue;
                        }
                        else if (t == typeof(BuiltInCommandMiddleware))
                        {
                            var m = new BuiltInCommandMiddleware(
                                next,
                                GetRequiredService<ICoconaHelpRenderer>(_serviceProvider),
                                GetRequiredService<ICoconaCommandHelpProvider>(_serviceProvider),
                                GetRequiredService<ICoconaCommandProvider>(_serviceProvider),
                                GetRequiredService<ICoconaConsoleProvider>(_serviceProvider),
                                GetRequiredService<ICoconaAppContextAccessor>(_serviceProvider),
                                GetRequiredService<ICoconaShellCompletionCodeGenerator>(_serviceProvider));
                            next = m.DispatchAsync;
                            continue;
                        }

                        var middleware = (CommandDispatcherMiddleware)_activator.CreateInstance(_serviceProvider, typeOrInstance.Type, new object[] { next })!;
                        next = middleware.DispatchAsync;
                    }
                }
            }

            return next;
        }

        private T GetRequiredService<T>(IServiceProvider serviceProvider)
        {
            return (T)(serviceProvider.GetService(typeof(T)) ?? new InvalidOperationException());
        }
    }
}
