using WildCoco.Application;
using WildCoco.Application;
using Microsoft.Extensions.DependencyInjection;

namespace WildCoco.Hosting;

public class WildCocoServiceProviderScopeSupport : IWildCocoServiceProviderScopeSupport
{
    public (IDisposable Scope, IServiceProvider ScopedServiceProvider) CreateScope(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        return (scope, scope.ServiceProvider);
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1
    public (IAsyncDisposable Scope, IServiceProvider ScopedServiceProvider) CreateAsyncScope(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateAsyncScope();
        return (scope, scope.ServiceProvider);
    }
#endif
}
