using Cocona.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Cocona.Builder.Internal;
using Cocona;
using Cocona.Builder;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

public static class CoconaHostApplicationBuilderExtensions
{
    /// <summary>
    /// Adds and configures a Cocona application.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="args"></param>
    /// <param name="types"></param>
    /// <param name="methods"></param>
    /// <returns></returns>
    public static HostApplicationBuilder ConfigureCocona(
        this HostApplicationBuilder hostBuilder, 
        string[]? args, 
        IEnumerable<Type> types, 
        IEnumerable<Delegate>? methods = null)
            => hostBuilder.ConfigureCocona(args, (app) =>
            {
                app.AddCommands(types);

                foreach (var method in methods ?? [])
                {
                    app.AddCommand(method);
                }
            });

    /// <summary>
    /// Adds and configures a Cocona application.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="args"></param>
    /// <param name="configureApplication"></param>
    /// <returns></returns>
    public static HostApplicationBuilder ConfigureCocona(
        this HostApplicationBuilder hostBuilder, 
        string[]? args, 
        Action<ICoconaCommandsBuilder>? configureApplication = null)
    {
        hostBuilder.Services.AddCoconaCore(args);
        hostBuilder.Services.AddCoconaShellCompletion();

        hostBuilder.Services.AddHostedService<CoconaHostedService>();

        hostBuilder.Services.Configure<CoconaAppHostOptions>(options =>
        {
            options.ConfigureApplication = app =>
            {
                configureApplication?.Invoke(app);
            };
        });
        return hostBuilder;
    }

}
