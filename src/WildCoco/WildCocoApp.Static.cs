using WildCoco;
using WildCoco.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WildCoco.Builder;
using WildCoco.Hosting;

namespace WildCoco;

public partial class WildCocoApp
{
    /// <summary>
    /// Creates an instance of <see cref="WildCocoAppHostBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public static WildCocoApp Create(string[]? args = default, Action<WildCocoAppOptions>? configureOptions = null)
        => new WildCocoAppBuilder(args).Build();

    /// <summary>
    /// Creates an instance of <see cref="WildCocoAppBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public static WildCocoAppBuilder CreateBuilder(string[]? args = default, Action<WildCocoAppOptions>? configureOptions = null)
        => new WildCocoAppBuilder(args, configureOptions);

    /// <summary>
    /// Creates an instance of <see cref="WildCocoAppHostBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public static WildCocoAppHostBuilder CreateHostBuilder()
        => new WildCocoAppHostBuilder(Host.CreateDefaultBuilder(null));

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static void Run<T>(string[]? args = null, Action<WildCocoAppOptions>? configureOptions = null)
        => Run(args, new[] { typeof(T) }, configureOptions);

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync<T>(string[]? args = null, Action<WildCocoAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        => RunAsync(args, new[] { typeof(T) }, configureOptions, cancellationToken);

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    public static void Run(Type[] commandTypes, Action<WildCocoAppOptions>? configureOptions = null)
        => Run(null, commandTypes, configureOptions);

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    public static void Run(string[]? args, Type[] commandTypes, Action<WildCocoAppOptions>? configureOptions = null)
    {
        var builder = CreateBuilder(args);
        if (configureOptions is not null)
        {
            builder.Services.Configure<WildCocoAppOptions>(configureOptions);
        }

        var app = builder.Build();
        app.AddCommands(commandTypes);

        app.Run();
    }

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    public static Task RunAsync(Type[] commandTypes, Action<WildCocoAppOptions>? configureOptions = null)
        => RunAsync(null, commandTypes, configureOptions);

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync(string[]? args, Type[] commandTypes, Action<WildCocoAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
    {
        var builder = CreateBuilder(args);
        if (configureOptions is not null)
        {
            builder.Services.Configure<WildCocoAppOptions>(configureOptions);
        }

        var app = builder.Build();
        app.AddCommands(commandTypes);

        return app.RunAsync(cancellationToken);
    }

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="commandBody"></param>
    /// <param name="args"></param>
    /// <param name="configureCommand"></param>
    /// <param name="configureOptions"></param>
    public static void Run(Delegate commandBody, string[]? args = null, Action<CommandConventionBuilder>? configureCommand = null, Action<WildCocoAppOptions>? configureOptions = null)
    {
        var builder = CreateBuilder(args);
        if (configureOptions is not null)
        {
            builder.Services.Configure<WildCocoAppOptions>(configureOptions);
        }

        var app = builder.Build();
        var commandConventionBuilder = app.AddCommand(commandBody);
        configureCommand?.Invoke(commandConventionBuilder);

        app.Run();
    }

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="commandBody"></param>
    /// <param name="args"></param>
    /// <param name="configureCommand"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync(Delegate commandBody, string[]? args = null, Action<CommandConventionBuilder>? configureCommand = null, Action<WildCocoAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
    {
        var builder = CreateBuilder(args);
        if (configureOptions is not null)
        {
            builder.Services.Configure<WildCocoAppOptions>(configureOptions);
        }

        var app = builder.Build();
        var commandConventionBuilder = app.AddCommand(commandBody);
        configureCommand?.Invoke(commandConventionBuilder);

        return app.RunAsync(cancellationToken);
    }
}
