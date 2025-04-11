using WildCoco;
using WildCoco.Builder;
using WildCoco.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WildCoco.Builder.Internal;
using WildCoco;
using WildCoco.Builder;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

public static class WildCocoHostBuilderExtensions
{
    /// <summary>
    /// Adds and configures a WildCoco application.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="args"></param>
    /// <param name="types"></param>
    /// <param name="methods"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureWildCoco(this IHostBuilder hostBuilder, string[]? args, IEnumerable<Type> types, IEnumerable<Delegate>? methods = null)
        => hostBuilder.ConfigureWildCoco(args, configureApplication: (app) =>
        {
            app.AddCommands(types);

            foreach (var method in (methods ?? Array.Empty<Delegate>()))
            {
                app.AddCommand(method);
            }
        });

    /// <summary>
    /// Adds and configures a WildCoco application.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="args"></param>
    /// <param name="configureHost"></param>
    /// <param name="configureApplication"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureWildCoco(this IHostBuilder hostBuilder, string[]? args, Action<IHostBuilder>? configureHost = null, Action<IWildCocoCommandsBuilder>? configureApplication = null)
    {
        configureHost?.Invoke(hostBuilder);

        return hostBuilder
            .ConfigureLogging(logging => { })
            .ConfigureAppConfiguration(config => { })
            .ConfigureServices(services =>
            {
                services.AddWildCocoCore(args);
                services.AddWildCocoShellCompletion();

                services.AddHostedService<WildCocoHostedService>();

                services.Configure<WildCocoAppHostOptions>(options =>
                {
                    options.ConfigureApplication = app =>
                    {
                        configureApplication?.Invoke(app);
                    };
                });
            });
    }

}
