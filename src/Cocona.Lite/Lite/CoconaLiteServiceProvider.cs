using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cocona.Application;

namespace Cocona.Lite
{
    public class CoconaLiteServiceProvider : IServiceProvider, IDisposable, ICoconaServiceProviderIsService
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
                return descriptors[0].Factory(this, _disposables);
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
    }
}
