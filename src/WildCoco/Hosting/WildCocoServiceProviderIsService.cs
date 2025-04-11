using WildCoco.Application;
using WildCoco.Application;
using Microsoft.Extensions.DependencyInjection;

namespace WildCoco.Hosting;

public class WildCocoServiceProviderIsService : IWildCocoServiceProviderIsService
{
    private readonly IServiceProviderIsService _serviceProviderIsService;
    public WildCocoServiceProviderIsService(IServiceProviderIsService serviceProviderIsService)
    {
        _serviceProviderIsService = serviceProviderIsService;
    }

    public bool IsService(Type t)
        => _serviceProviderIsService.IsService(t);
}
