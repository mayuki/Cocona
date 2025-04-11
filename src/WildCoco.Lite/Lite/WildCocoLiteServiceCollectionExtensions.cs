namespace WildCoco.Lite;

public static class WildCocoLiteServiceCollectionExtensions
{
    public static void AddDescriptor(this IWildCocoLiteServiceCollection services, ServiceDescriptor serviceDescriptor)
    {
        services.Add(serviceDescriptor);
    }

    public static void AddDescriptor<TService>(this IWildCocoLiteServiceCollection services, Func<IServiceProvider, List<IDisposable>, object> factory, bool singleton)
    {
        services.AddDescriptor(new ServiceDescriptor(typeof(TService), factory, singleton));
    }

    public static void AddTransient<TService>(this IWildCocoLiteServiceCollection services, Func<IServiceProvider, TService> factory)
    {
        services.AddDescriptor<TService>((provider, disposables) =>
        {
            var instance = factory(provider) ?? throw new InvalidOperationException($"The service factory of '{typeof(TService)}' must be non-null value.");
            if (instance is IDisposable disposable)
            {
                disposables.Add(disposable);
            }

            return instance;
        }, singleton: false);
    }

    public static void AddTransient<TService, TImplementation>(this IWildCocoLiteServiceCollection services)
        where TImplementation : TService
    {
        services.AddDescriptor<TService>((provider, disposables) =>
        {
            var instance = SimpleActivator.CreateInstance(provider, typeof(TImplementation));
            if (instance is IDisposable disposable)
            {
                disposables.Add(disposable);
            }

            return instance;
        }, singleton: false);
    }

    public static void AddSingleton<TService, TImplementation>(this IWildCocoLiteServiceCollection services)
        where TImplementation : TService
    {
        services.AddDescriptor<TService>((provider, disposables) =>
        {
            var instance = (TService)SimpleActivator.CreateInstance(provider, typeof(TImplementation))
                           ?? throw new InvalidOperationException($"The service factory of '{typeof(TService)}' must be non-null value.");
            if (instance is IDisposable disposable)
            {
                disposables.Add(disposable);
            }
            return instance;
        }, singleton: true);
    }

    public static void AddSingleton<TService>(this IWildCocoLiteServiceCollection services, TService instance)
    {
        services.AddSingleton<TService>(_ => instance);
    }

    public static void AddSingleton<TService>(this IWildCocoLiteServiceCollection services, Func<IServiceProvider, TService> factory)
    {
        services.AddDescriptor<TService>((provider, disposables) =>
        {
            var instance = factory(provider) ?? throw new InvalidOperationException($"The service factory of '{typeof(TService)}' must be non-null value.");
            if (instance is IDisposable disposable)
            {
                disposables.Add(disposable);
            }
            return instance;
        }, singleton: true);
    }

    public static void TryAddTransient<TService>(this IWildCocoLiteServiceCollection services, Func<IServiceProvider, TService> factory)
    {
        if (services.All(x => x.ServiceType != typeof(TService)))
        {
            services.AddTransient<TService>(factory);
        }
    }

    public static void TryAddTransient<TService, TImplementation>(this IWildCocoLiteServiceCollection services)
        where TImplementation : TService
    {
        if (services.All(x => x.ServiceType != typeof(TService)))
        {
            services.AddTransient<TService, TImplementation>();
        }
    }

    public static void TryAddSingleton<TService, TImplementation>(this IWildCocoLiteServiceCollection services)
        where TImplementation : TService
    {
        if (services.All(x => x.ServiceType != typeof(TService)))
        {
            services.AddSingleton<TService, TImplementation>();
        }
    }

    public static void TryAddSingleton<TService>(this IWildCocoLiteServiceCollection services, TService instance)
    {
        if (services.All(x => x.ServiceType != typeof(TService)))
        {
            services.AddSingleton<TService>(instance);
        }
    }

    public static void TryAddSingleton<TService>(this IWildCocoLiteServiceCollection services, Func<IServiceProvider, TService> factory)
    {
        if (services.All(x => x.ServiceType != typeof(TService)))
        {
            services.AddSingleton<TService>(factory);
        }
    }

}
