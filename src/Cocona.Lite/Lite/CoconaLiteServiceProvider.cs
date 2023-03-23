using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cocona.Application;

namespace Cocona.Lite
{
    public class CoconaLiteServiceProvider : IServiceProvider, IDisposable, ICoconaServiceProviderIsService, ICoconaServiceProviderScopeSupport
    {
        private readonly Dictionary<Type, ServiceDescriptor[]> _descriptorsByService;
        private readonly List<IDisposable> _disposables;

        public CoconaLiteServiceProvider(ICoconaLiteServiceCollection services)
        {
            _descriptorsByService = services.GroupBy(k => k.ServiceType).ToDictionary(k => k.Key, v => v.ToArray());
            _disposables = new List<IDisposable>(10);
        }

        public bool IsService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider)) return true;
            if (serviceType == typeof(ICoconaServiceProviderIsService)) return true;
            if (serviceType == typeof(ICoconaServiceProviderScopeSupport)) return true;

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
            if (serviceType == typeof(ICoconaServiceProviderIsService)) return this;
            if (serviceType == typeof(ICoconaServiceProviderScopeSupport)) return this;

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

        // NOTE: Cocona.Lite's ServiceProvider does not support `Scoped`.
        (IDisposable Scope, IServiceProvider ScopedServiceProvider) ICoconaServiceProviderScopeSupport.CreateScope(IServiceProvider serviceProvider)
            => (new NullDisposable(), serviceProvider);

#if NET5_0_OR_GREATER || NETSTANDARD2_1
        (IAsyncDisposable Scope, IServiceProvider ScopedServiceProvider) ICoconaServiceProviderScopeSupport.CreateAsyncScope(IServiceProvider serviceProvider)
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
}
