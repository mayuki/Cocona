using System;
using Cocona.Application;

namespace Cocona.Lite
{
    public class CoconaLiteInstanceActivator : ICoconaInstanceActivator
    {
        public object? GetServiceOrCreateInstance(IServiceProvider serviceProvider, Type instanceType)
        {
            return serviceProvider.GetService(instanceType) ?? CreateInstance(serviceProvider, instanceType, Array.Empty<object>());
        }

        public object? CreateInstance(IServiceProvider serviceProvider, Type instanceType, object[]? parameters)
        {
            if (parameters != null && parameters.Length > 0) throw new NotSupportedException("SimpleCoconaInstanceActivator doesn't support extra arguments.");
            return SimpleActivator.CreateInstance(serviceProvider, instanceType);
        }
    }
}
