namespace Cocona;

public static class ServiceProviderExtensions
{
    public static T? GetService<T>(this IServiceProvider provider)
    {
        return (T?)provider.GetService(typeof(T));
    }
        
    public static object GetRequiredService(this IServiceProvider provider, Type t)
    {
        return provider.GetService(t) ?? throw new InvalidOperationException($"No service for type '{t}' has been registered.");
    }
        
    public static T GetRequiredService<T>(this IServiceProvider provider)
    {
        return (T)provider.GetRequiredService(typeof(T));
    }
}
