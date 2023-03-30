using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cocona.Builder.Internal;

internal class LoggingBuilder : ILoggingBuilder
{
    private readonly IServiceCollection _services;
    public LoggingBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public IServiceCollection Services => _services;
}
