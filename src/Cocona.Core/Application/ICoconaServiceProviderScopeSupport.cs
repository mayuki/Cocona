namespace Cocona.Application
{
    public interface ICoconaServiceProviderScopeSupport
    {
        (IDisposable Scope, IServiceProvider ScopedServiceProvider) CreateScope(IServiceProvider serviceProvider);
#if NET5_0_OR_GREATER || NETSTANDARD2_1
        (IAsyncDisposable Scope, IServiceProvider ScopedServiceProvider) CreateAsyncScope(IServiceProvider serviceProvider);
#endif
    }
}
