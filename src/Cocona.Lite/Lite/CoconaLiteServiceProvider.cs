using System;
using System.Collections.Generic;

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
        private readonly Dictionary<Type, Func<IServiceProvider, object>> _factories = new Dictionary<Type, Func<IServiceProvider, object>>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>(10);

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider)) return this;

            if (_factories.TryGetValue(serviceType, out var factory))
            {
                return factory(this);
            }

            return null!;
        }

        public void AddTransient<TService, TImplementation>()
            where TImplementation : TService
        {
            _factories[typeof(TService)] = provider =>
            {
                var instance = SimpleActivator.CreateInstance(this, typeof(TImplementation));
                if (instance is IDisposable disposable)
                {
                    _disposables.Add(disposable);
                }

                return instance;
            };
        }

        public void AddSingleton<TService, TImplementation>()
            where TImplementation : TService
        {
            _factories[typeof(TService)] = provider =>
            {
                var instance = (TService)SimpleActivator.CreateInstance(this, typeof(TImplementation));
                if (instance is IDisposable disposable)
                {
                    _disposables.Add(disposable);
                }
                _factories[typeof(TService)] = _ => instance!;
                return instance!;
            };
        }

        public void AddSingleton<TService>(TService instance)
        {
            _factories[typeof(TService)] = _ => instance!;
        }

        public void AddSingleton<TService>(Func<IServiceProvider, TService> factory)
        {
            _factories[typeof(TService)] = provider =>
            {
                var instance = factory(this);
                if (instance is IDisposable disposable)
                {
                    _disposables.Add(disposable);
                }
                _factories[typeof(TService)] = _ => instance!;
                return instance!;
            };
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
