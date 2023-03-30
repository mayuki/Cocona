using Microsoft.Extensions.DependencyInjection;

namespace Cocona.Application;

public class CoconaInstanceActivator : ICoconaInstanceActivator
{
    public object? GetServiceOrCreateInstance(IServiceProvider serviceProvider, Type instanceType)
    {
        return ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, instanceType);
    }

    public object? CreateInstance(IServiceProvider serviceProvider, Type instanceType, object[]? parameters)
    {
        return ActivatorUtilities.CreateInstance(serviceProvider, instanceType, parameters ?? Array.Empty<object>());
    }
}
