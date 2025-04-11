using WildCoco.Application;
using Microsoft.Extensions.DependencyInjection;

namespace WildCoco.Application;

public class WildCocoInstanceActivator : IWildCocoInstanceActivator
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
