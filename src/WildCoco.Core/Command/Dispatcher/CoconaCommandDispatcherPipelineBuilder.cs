using WildCoco.Application;
using WildCoco.Command.Binder;
using WildCoco.Command.Dispatcher.Middlewares;

namespace WildCoco.Command.Dispatcher;

public delegate ValueTask<int> CommandDispatchDelegate(CommandDispatchContext ctx);

public class WildCocoCommandDispatcherPipelineBuilder : IWildCocoCommandDispatcherPipelineBuilder
{
    private readonly List<(Type? Type, object? Instance)> _typesOrInstances = new List<(Type? Type, object? Instance)>();
    private readonly IServiceProvider _serviceProvider;
    private readonly IWildCocoInstanceActivator _activator;

    public WildCocoCommandDispatcherPipelineBuilder(IServiceProvider serviceProvider, IWildCocoInstanceActivator activator)
    {
        _serviceProvider = serviceProvider;
        _activator = activator;
    }

    public IWildCocoCommandDispatcherPipelineBuilder UseMiddleware<T>()
        where T: CommandDispatcherMiddleware
    {
        var t = typeof(T);

        if (t.IsAbstract || (t.IsGenericType && !t.IsConstructedGenericType))
            throw new ArgumentException("A middleware type must be constructed class.");

        _typesOrInstances.Add((t, null));
        return this;
    }

    public IWildCocoCommandDispatcherPipelineBuilder UseMiddleware(Func<CommandDispatchDelegate, CommandDispatchContext, ValueTask<int>> middleware)
    {
        _typesOrInstances.Add((null, middleware));
        return this;
    }

    public IWildCocoCommandDispatcherPipelineBuilder UseMiddleware(Func<CommandDispatchDelegate, IServiceProvider, CommandDispatcherMiddleware> factory)
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
                    if (t == typeof(WildCocoCommandInvokeMiddleware))
                    {
                        var m = new WildCocoCommandInvokeMiddleware(next, GetRequiredService<IWildCocoParameterBinder>(_serviceProvider));
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
                        var m = new HandleExceptionAndExitMiddleware(next, GetRequiredService<IWildCocoConsoleProvider>(_serviceProvider));
                        next = m.DispatchAsync;
                        continue;
                    }
                    else if (t == typeof(HandleParameterBindExceptionMiddleware))
                    {
                        var m = new HandleParameterBindExceptionMiddleware(next, GetRequiredService<IWildCocoConsoleProvider>(_serviceProvider));
                        next = m.DispatchAsync;
                        continue;
                    }
                    else if (t == typeof(RejectUnknownOptionsMiddleware))
                    {
                        var m = new RejectUnknownOptionsMiddleware(next, GetRequiredService<IWildCocoConsoleProvider>(_serviceProvider));
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
