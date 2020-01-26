using System;

namespace Cocona.Application
{
    public interface ICoconaInstanceActivator
    {
        object? GetServiceOrCreateInstance(IServiceProvider serviceProvider, Type instanceType);
        object? CreateInstance(IServiceProvider serviceProvider, Type instanceType, object[]? parameters);
    }
}
