using WildCoco;
using WildCoco.Builder;
using WildCoco.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WildCoco.Hosting;

/// <summary>
/// Builder for WildCoco enabled application host.
/// </summary>
public class WildCocoAppHostBuilder
{
    private readonly IHostBuilder _builder;

    public WildCocoAppHostBuilder(IHostBuilder hostBuilder)
    {
        _builder = hostBuilder;
    }

    /// <summary>
    /// Adds services to the container. See also <seealso cref="HostingHostBuilderExtensions.ConfigureServices(IHostBuilder, Action{IServiceCollection})"/>.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public WildCocoAppHostBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
    {
        _builder.ConfigureServices(configureDelegate);
        return this;
    }

    /// <summary>
    /// Adds services to the container. See also <seealso cref="HostingHostBuilderExtensions.ConfigureServices(IHostBuilder, Action{IServiceCollection})"/>.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public WildCocoAppHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        _builder.ConfigureServices(configureDelegate);
        return this;
    }

    /// <summary>
    /// Sets up the configuration for the application. See also <seealso cref="HostingHostBuilderExtensions.ConfigureAppConfiguration(IHostBuilder, Action{IConfigurationBuilder})"/>.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public WildCocoAppHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        _builder.ConfigureAppConfiguration(configureDelegate);
        return this;
    }

    /// <summary>
    /// Sets up the configuration for the application. See also <seealso cref="HostingHostBuilderExtensions.ConfigureAppConfiguration(IHostBuilder, Action{IConfigurationBuilder})"/>.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public WildCocoAppHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        _builder.ConfigureAppConfiguration(configureDelegate);
        return this;
    }

    /// <summary>
    /// Adds a delegate for configuring the provided <see cref="ILoggingBuilder"/>. This may be called multiple times. See also <seealso cref="HostingHostBuilderExtensions.ConfigureLogging(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.Extensions.Logging.ILoggingBuilder})"/>.
    /// </summary>
    /// <param name="configureLogging"></param>
    /// <returns></returns>
    public WildCocoAppHostBuilder ConfigureLogging(Action<ILoggingBuilder> configureLogging)
    {
        _builder.ConfigureLogging(configureLogging);
        return this;
    }
        
    /// <summary>
    /// Adds a delegate for configuring the provided <see cref="ILoggingBuilder"/>. This may be called multiple times. See also <seealso cref="HostingHostBuilderExtensions.ConfigureLogging(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.Extensions.Hosting.HostBuilderContext,Microsoft.Extensions.Logging.ILoggingBuilder})"/>.
    /// </summary>
    /// <param name="configureLogging"></param>
    /// <returns></returns>
    public WildCocoAppHostBuilder ConfigureLogging(Action<HostBuilderContext, ILoggingBuilder> configureLogging)
    {
        _builder.ConfigureLogging(configureLogging);
        return this;
    }

    internal void ConfigureDefaults(string[]? args, Action<IWildCocoCommandsBuilder> configureApplication)
    {
        _builder
            .ConfigureWildCoco(args, configureApplication: configureApplication)
            .UseConsoleLifetime(options => options.SuppressStatusMessages = true);
    }

    private IHost Build(string[]? args, Type[] types, Action<WildCocoAppOptions>? configureOptions)
    {
        ConfigureDefaults(args, app =>
        {
            app.AddCommands(types);
        });

        return _builder
            .ConfigureServices(services =>
            {
                if (configureOptions != null)
                {
                    services.Configure(configureOptions);
                }
            })
            .Build();
    }

    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    public void Run(string[]? args, Type[] commandTypes, Action<WildCocoAppOptions>? configureOptions = null)
        => Build(args, commandTypes, configureOptions).Run();

    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task RunAsync(string[]? args, Type[] commandTypes, Action<WildCocoAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        => Build(args, commandTypes, configureOptions).RunAsync(cancellationToken);
}
