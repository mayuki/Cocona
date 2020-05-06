using System;
using System.Collections.Generic;
using System.Linq;

namespace Cocona.Lite
{
    public interface ICoconaLiteServiceCollection
    {
        void AddTransient<TService, TImplementation>()
            where TImplementation : TService;

        void AddSingleton<TService, TImplementation>()
            where TImplementation : TService;

        void AddSingleton<TService>(TService instance);

        void AddSingleton<TService>(Func<IServiceProvider, TService> factory);
    }

    public class CoconaLiteServiceProvider : IServiceProvider, ICoconaLiteServiceCollection, IDisposable
    {
        private readonly Dictionary<Type, List<ServiceDescriptor>> _descriptorsByService = new Dictionary<Type, List<ServiceDescriptor>>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>(10);

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider)) return this;

            // IEnumerable<T>
            if (serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return GetServiceForArray(serviceType.GetGenericArguments()[0]);
            }
            // T[]
            if (serviceType.IsArray)
            {
                return GetServiceForArray(serviceType.GetElementType());
            }

            if (_descriptorsByService.TryGetValue(serviceType, out var descriptors) && descriptors.Count != 0)
            {
                return descriptors[0].Factory(this);
            }

            return null!;
        }

        private void AddDescriptor<TService>(Func<IServiceProvider, object> factory, bool singleton)
        {
            if (!_descriptorsByService.ContainsKey(typeof(TService)))
            {
                _descriptorsByService[typeof(TService)] = new List<ServiceDescriptor>();
            }

            var descriptor = new ServiceDescriptor(factory, singleton);
            _descriptorsByService[typeof(TService)].Add(descriptor);
        }

        private object GetServiceForArray(Type elementType)
        {
            if (_descriptorsByService.TryGetValue(elementType, out var descriptors) && descriptors.Count != 0)
            {
                var index = 0;
                var typedArr = Array.CreateInstance(elementType, descriptors.Count);
                foreach (var descriptor in descriptors)
                {
                    typedArr.SetValue(descriptor.Factory(this), index++);
                }

                return typedArr;
            }

            return Array.CreateInstance(elementType, 0);
        }

        public void AddTransient<TService, TImplementation>()
            where TImplementation : TService
        {
            AddDescriptor<TService>((provider) =>
            {
                var instance = SimpleActivator.CreateInstance(this, typeof(TImplementation));
                if (instance is IDisposable disposable)
                {
                    _disposables.Add(disposable);
                }

                return instance;
            }, singleton: false);
        }

        public void AddSingleton<TService, TImplementation>()
            where TImplementation : TService
        {
            AddDescriptor<TService>((provider) =>
            {
                var instance = (TService)SimpleActivator.CreateInstance(this, typeof(TImplementation));
                if (instance is IDisposable disposable)
                {
                    _disposables.Add(disposable);
                }
                return instance!;
            }, singleton: true);
        }

        public void AddSingleton<TService>(TService instance)
        {
            AddDescriptor<TService>(_ => instance!, singleton: true);
        }

        public void AddSingleton<TService>(Func<IServiceProvider, TService> factory)
        {
            AddDescriptor<TService>((provider) =>
            {
                var instance = factory(this);
                if (instance is IDisposable disposable)
                {
                    _disposables.Add(disposable);
                }
                return instance!;
            }, singleton: true);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        private class ServiceDescriptor
        {
            public Func<IServiceProvider, object> Factory { get; private set; }

            public ServiceDescriptor(Func<IServiceProvider, object> factory, bool singleton)
            {
                if (singleton)
                {
                    Factory = services =>
                    {
                        var instance = factory(services);
                        Factory = _ => instance;
                        return instance;
                    };
                }
                else
                {
                    Factory = factory;
                }
            }
        }
    }
}
