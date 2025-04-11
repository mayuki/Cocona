using WildCoco;
using WildCoco.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WildCoco.Builder.Internal;
using WildCoco.Hosting;

namespace WildCoco.Builder;

/// <summary>
/// A builder for console applications.
/// </summary>
public class WildCocoAppBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly HostBuilder _hostBuilder = new HostBuilder();
    private readonly ConfigureHostBuilder _configureHostBuilder;
    private readonly Action<WildCocoAppOptions>? _configureOptions;
    private WildCocoApp? _application;

    public IServiceCollection Services => _services;
    public ILoggingBuilder Logging { get; }
    public IHostBuilder Host => _configureHostBuilder;
    public IHostEnvironment Environment { get; }
    public ConfigurationManager Configuration { get; }

    internal WildCocoAppBuilder(string[]? args, Action<WildCocoAppOptions>? configureOptions = null)
    {
        Configuration = new ConfigurationManager();
        _configureOptions = configureOptions;

        // Sets the default configuration values for the application host.
        // such as EnvironmentName, ApplicationName, ContentRoot...
        var bootstrapHostBuilder = new BootstrapHostBuilder(_services);
        bootstrapHostBuilder.ConfigureDefaultWildCoco(args, app =>
        {
            // Copy commands from WildCocoApp to WildCocoAppHostOptions on starting application.
            foreach (var commandData in ((IWildCocoCommandsBuilder)_application!).Build())
            {
                app.AddCommand(commandData);
            }
        });
        var (hostBuilderContext, hostConfiguration) = bootstrapHostBuilder.Apply(Configuration, _hostBuilder);

        _configureHostBuilder = new ConfigureHostBuilder(hostBuilderContext, Configuration, Services);
        Environment = hostBuilderContext.HostingEnvironment;
        Logging = new LoggingBuilder(_services);

        _services.AddSingleton<IConfiguration>(sp => Configuration);
    }

    public WildCocoApp Build()
    {
        _hostBuilder.ConfigureAppConfiguration((hostBuilder, configuration) =>
        {
            // Use the HostEnvironment created by WildCocoAppBuilder instead of the default.
            hostBuilder.HostingEnvironment = Environment;

            var chainedSource = new ChainedConfigurationSource()
            {
                Configuration = Configuration,
                ShouldDisposeConfiguration = true,
            };
            configuration.Add(chainedSource);

            foreach (var keyValue in ((IConfigurationBuilder)Configuration).Properties)
            {
                configuration.Properties[keyValue.Key] = keyValue.Value;
            }
        });

        _hostBuilder.ConfigureServices((hostBuilder, services) =>
        {
            services.AddSingleton(Environment);

            if (_configureOptions != null)
            {
                services.Configure<WildCocoAppOptions>(_configureOptions);
            }

            foreach (var service in _services.Where(x => !typeof(IHostedService).IsAssignableFrom(x.ServiceType)))
            {
                services.Add(service);
            }
            foreach (var service in _services.Where(x => typeof(IHostedService).IsAssignableFrom(x.ServiceType)))
            {
                services.Add(service);
            }
        });

        _configureHostBuilder.RunOperations(_hostBuilder);

        _application = new WildCocoApp(_hostBuilder.Build());
        return _application;
    }
}

internal static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureDefaultWildCoco(this IHostBuilder hostBuilder, string[]? args, Action<IWildCocoCommandsBuilder> configureApplication)
    {
        var builder = new WildCocoAppHostBuilder(hostBuilder);
        builder.ConfigureDefaults(args, configureApplication);

        return hostBuilder;
    }
}
