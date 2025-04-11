namespace WildCoco.Application;

public interface IWildCocoInstanceActivator
{
    object? GetServiceOrCreateInstance(IServiceProvider serviceProvider, Type instanceType);
    object? CreateInstance(IServiceProvider serviceProvider, Type instanceType, object[]? parameters);
}
