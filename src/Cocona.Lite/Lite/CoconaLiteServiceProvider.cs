using System;
using System.Collections.Generic;

namespace Cocona.Lite
{
    public class CoconaLiteServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, Func<IServiceProvider, object>> _factories = new Dictionary<Type, Func<IServiceProvider, object>>();

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
            _factories[typeof(TService)] = provider => SimpleActivator.CreateInstance(this, typeof(TImplementation));
        }

        public void AddSingleton<TService, TImplementation>()
            where TImplementation : TService
        {
            AddSingleton(_ => (TService)SimpleActivator.CreateInstance(this, typeof(TImplementation)));
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
                _factories[typeof(TService)] = _ => instance!;
                return instance!;
            };
        }

    }
}
