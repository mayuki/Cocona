using WildCoco.Builder;
using WildCoco.Lite.Hosting;

namespace WildCoco.Lite.Builder;

/// <summary>
/// A builder for console applications.
/// </summary>
public class WildCocoLiteAppBuilder
{
    private readonly WildCocoLiteAppHostBuilder _hostBuilder;
    private readonly IWildCocoLiteServiceCollection _services;
    private WildCocoLiteApp? _application;

    public IWildCocoLiteServiceCollection Services => _services;

    internal WildCocoLiteAppBuilder(string[]? args, Action<WildCocoLiteAppOptions>? configure)
    {
        _services = new WildCocoLiteServiceCollection();

        _hostBuilder = new WildCocoLiteAppHostBuilder(args);
        _hostBuilder.ConfigureOptions(configure);
        _hostBuilder.ConfigureApplication(app =>
        {
            // Copy commands from WildCocoApp to WildCocoAppHostOptions on starting application.
            foreach (var commandData in ((IWildCocoCommandsBuilder)_application!).Build())
            {
                app.AddCommand(commandData);
            }
        });
    }

    public WildCocoLiteApp Build()
    {
        _hostBuilder.ConfigureServices((services) =>
        {
            foreach (var service in _services)
            {
                services.Add(service);
            }
        });

        _application = new WildCocoLiteApp(_hostBuilder.Build());
        return _application;
    }
}
