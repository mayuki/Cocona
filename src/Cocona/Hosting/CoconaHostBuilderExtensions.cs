using Cocona.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Cocona.Builder.Internal;
using Cocona;
using Cocona.Builder;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

public static class CoconaHostBuilderExtensions
{
    /// <summary>
    /// Adds and configures a Cocona application.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="args"></param>
    /// <param name="types"></param>
    /// <param name="methods"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureCocona(this IHostBuilder hostBuilder, string[]? args, IEnumerable<Type> types, IEnumerable<Delegate>? methods = null)
        => hostBuilder.ConfigureCocona(args, configureApplication: (app) =>
        {
            app.AddCommands(types);

            foreach (var method in (methods ?? Array.Empty<Delegate>()))
            {
                app.AddCommand(method);
            }
        });

    /// <summary>
    /// Adds and configures a Cocona application.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="args"></param>
    /// <param name="configureHost"></param>
    /// <param name="configureApplication"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureCocona(this IHostBuilder hostBuilder, string[]? args, Action<IHostBuilder>? configureHost = null, Action<ICoconaCommandsBuilder>? configureApplication = null)
    {
        configureHost?.Invoke(hostBuilder);

        return hostBuilder
            .ConfigureLogging(logging => { })
            .ConfigureAppConfiguration(config => { })
            .ConfigureServices(services =>
            {
                services.AddCoconaCore(args);
                services.AddCoconaShellCompletion();

                services.AddHostedService<CoconaHostedService>();

                services.Configure<CoconaAppHostOptions>(options =>
                {
                    options.ConfigureApplication = app =>
                    {
                        configureApplication?.Invoke(app);
                    };
                });
            });
    }

}
