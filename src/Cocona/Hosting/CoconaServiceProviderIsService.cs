using Cocona.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Cocona.Hosting
{
    public class CoconaServiceProviderIsService : ICoconaServiceProviderIsService
    {
        private readonly IServiceProviderIsService _serviceProviderIsService;
        public CoconaServiceProviderIsService(IServiceProviderIsService serviceProviderIsService)
        {
            _serviceProviderIsService = serviceProviderIsService;
        }

        public bool IsService(Type t)
            => _serviceProviderIsService.IsService(t);
    }
}
