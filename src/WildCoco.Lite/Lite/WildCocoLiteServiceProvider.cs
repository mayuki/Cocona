using WildCoco.Application;
using WildCoco.Application;

namespace WildCoco.Lite;

public class WildCocoLiteServiceProvider : IServiceProvider, IDisposable, IWildCocoServiceProviderIsService, IWildCocoServiceProviderScopeSupport
{
    private readonly Dictionary<Type, ServiceDescriptor[]> _descriptorsByService;
    private readonly List<IDisposable> _disposables;

    public WildCocoLiteServiceProvider(IWildCocoLiteServiceCollection services)
    {
        _descriptorsByService = services.GroupBy(k => k.ServiceType).ToDictionary(k => k.Key, v => v.ToArray());
        _disposables = new List<IDisposable>(10);
    }

    public bool IsService(Type serviceType)
    {
        if (serviceType == typeof(IServiceProvider)) return true;
        if (serviceType == typeof(IWildCocoServiceProviderIsService)) return true;
        if (serviceType == typeof(IWildCocoServiceProviderScopeSupport)) return true;

        // IEnumerable<T>
        if (serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return true;
        }
        // T[]
        if (serviceType.IsArray)
        {
            return true;
        }

        if (_descriptorsByService.TryGetValue(serviceType, out var descriptors) && descriptors.Length != 0)
        {
            return true;
        }

        return false;
    }

    public object GetService(Type serviceType)
    {
        if (serviceType == typeof(IServiceProvider)) return this;
        if (serviceType == typeof(IWildCocoServiceProviderIsService)) return this;
        if (serviceType == typeof(IWildCocoServiceProviderScopeSupport)) return this;

        // IEnumerable<T>
        if (serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return GetServiceForArray(serviceType.GetGenericArguments()[0]);
        }
        // T[]
        if (serviceType.IsArray)
        {
            return GetServiceForArray(serviceType.GetElementType()!);
        }

        if (_descriptorsByService.TryGetValue(serviceType, out var descriptors) && descriptors.Length != 0)
        {
            return descriptors[descriptors.Length - 1].Factory(this, _disposables);
        }

        return null!;
    }

    private object GetServiceForArray(Type elementType)
    {
        if (_descriptorsByService.TryGetValue(elementType, out var descriptors) && descriptors.Length != 0)
        {
            var index = 0;
            var typedArr = Array.CreateInstance(elementType, descriptors.Length);
            foreach (var descriptor in descriptors)
            {
                typedArr.SetValue(descriptor.Factory(this, _disposables), index++);
            }

            return typedArr;
        }

        return Array.CreateInstance(elementType, 0);
    }


    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }

        _disposables.Clear();
    }

    // NOTE: WildCoco.Lite's ServiceProvider does not support `Scoped`.
    (IDisposable Scope, IServiceProvider ScopedServiceProvider) IWildCocoServiceProviderScopeSupport.CreateScope(IServiceProvider serviceProvider)
        => (new NullDisposable(), serviceProvider);

#if NET5_0_OR_GREATER || NETSTANDARD2_1
    (IAsyncDisposable Scope, IServiceProvider ScopedServiceProvider) IWildCocoServiceProviderScopeSupport.CreateAsyncScope(IServiceProvider serviceProvider)
        => (new NullDisposable(), serviceProvider);
#endif

    private class NullDisposable :
        IDisposable
#if NET5_0_OR_GREATER || NETSTANDARD2_1
        , IAsyncDisposable
#endif
    {
        public void Dispose() {}
        public ValueTask DisposeAsync() => default;
    }
}
