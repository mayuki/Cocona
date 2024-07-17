using System.Diagnostics.CodeAnalysis;

namespace Cocona.Lite;

public static class CoconaLiteServiceCollectionExtensions
{
    public static void AddDescriptor(this ICoconaLiteServiceCollection services, ServiceDescriptor serviceDescriptor)
    {
        services.Add(serviceDescriptor);
    }

    public static void AddDescriptor<TService>(this ICoconaLiteServiceCollection services, Func<IServiceProvider, List<IDisposable>, object> factory, bool singleton)
    {
        services.AddDescriptor(new ServiceDescriptor(typeof(TService), factory, singleton));
    }

    public static void AddTransient<TService>(this ICoconaLiteServiceCollection services, Func<IServiceProvider, TService> factory)
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

    public static void AddTransient<TService,
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    TImplementation>(this ICoconaLiteServiceCollection services)
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

    public static void AddSingleton<TService,
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    TImplementation>(this ICoconaLiteServiceCollection services)
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

    public static void AddSingleton<TService>(this ICoconaLiteServiceCollection services, TService instance)
    {
        services.AddSingleton<TService>(_ => instance);
    }

    public static void AddSingleton<TService>(this ICoconaLiteServiceCollection services, Func<IServiceProvider, TService> factory)
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

    public static void TryAddTransient<TService>(this ICoconaLiteServiceCollection services, Func<IServiceProvider, TService> factory)
    {
        if (services.All(x => x.ServiceType != typeof(TService)))
        {
            services.AddTransient<TService>(factory);
        }
    }

    public static void TryAddTransient<TService,
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    TImplementation>(this ICoconaLiteServiceCollection services)
        where TImplementation : TService
    {
        if (services.All(x => x.ServiceType != typeof(TService)))
        {
            services.AddTransient<TService, TImplementation>();
        }
    }

    public static void TryAddSingleton<TService,
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    TImplementation>(this ICoconaLiteServiceCollection services)
        where TImplementation : TService
    {
        if (services.All(x => x.ServiceType != typeof(TService)))
        {
            services.AddSingleton<TService, TImplementation>();
        }
    }

    public static void TryAddSingleton<TService>(this ICoconaLiteServiceCollection services, TService instance)
    {
        if (services.All(x => x.ServiceType != typeof(TService)))
        {
            services.AddSingleton<TService>(instance);
        }
    }

    public static void TryAddSingleton<TService>(this ICoconaLiteServiceCollection services, Func<IServiceProvider, TService> factory)
    {
        if (services.All(x => x.ServiceType != typeof(TService)))
        {
            services.AddSingleton<TService>(factory);
        }
    }

}
