using WildCoco;
using WildCoco.Builder;
using WildCoco.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WildCoco.Builder.Internal;
using WildCoco;
using WildCoco.Builder;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

public static class WildCocoHostApplicationBuilderExtensions
{
    /// <summary>
    /// Adds and configures a WildCoco application.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="args"></param>
    /// <param name="types"></param>
    /// <param name="methods"></param>
    /// <returns></returns>
    public static HostApplicationBuilder ConfigureWildCoco(
        this HostApplicationBuilder hostBuilder, 
        string[]? args, 
        IEnumerable<Type> types, 
        IEnumerable<Delegate>? methods = null)
            => hostBuilder.ConfigureWildCoco(args, (app) =>
            {
                app.AddCommands(types);

                foreach (var method in methods ?? [])
                {
                    app.AddCommand(method);
                }
            });

    /// <summary>
    /// Adds and configures a WildCoco application.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="args"></param>
    /// <param name="configureApplication"></param>
    /// <returns></returns>
    public static HostApplicationBuilder ConfigureWildCoco(
        this HostApplicationBuilder hostBuilder, 
        string[]? args, 
        Action<IWildCocoCommandsBuilder>? configureApplication = null)
    {
        hostBuilder.Services.AddWildCocoCore(args);
        hostBuilder.Services.AddWildCocoShellCompletion();

        hostBuilder.Services.AddHostedService<WildCocoHostedService>();

        hostBuilder.Services.Configure<WildCocoAppHostOptions>(options =>
        {
            options.ConfigureApplication = app =>
            {
                configureApplication?.Invoke(app);
            };
        });
        return hostBuilder;
    }

}
