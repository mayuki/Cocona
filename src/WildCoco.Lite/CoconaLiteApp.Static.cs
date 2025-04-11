using WildCoco.Builder;
using WildCoco.Lite.Builder;
using WildCoco.Lite.Hosting;
using WildCoco.Builder;
using WildCoco.Lite.Builder;
using WildCoco.Lite.Hosting;

namespace WildCoco;

public partial class WildCocoLiteApp
{
    /// <summary>
    /// Creates an instance of <see cref="WildCocoLiteAppHostBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public static WildCocoLiteApp Create(string[]? args = null, Action<WildCocoLiteAppOptions>? configureOptions = null)
        => CreateBuilder(args, configureOptions).Build();

    /// <summary>
    /// Creates an instance of <see cref="WildCocoLiteAppBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public static WildCocoLiteAppBuilder CreateBuilder(string[]? args = null, Action<WildCocoLiteAppOptions>? configureOptions = null)
        => new WildCocoLiteAppBuilder(args, configureOptions);

    /// <summary>
    /// Creates an instance of <see cref="WildCocoLiteAppHostBuilder"/>.
    /// </summary>
    /// <returns></returns>
    public static WildCocoLiteAppHostBuilder CreateHostBuilder()
        => new WildCocoLiteAppHostBuilder(Array.Empty<string>());

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static void Run<T>(string[]? args = null, Action<WildCocoLiteAppOptions>? configureOptions = null)
        => Run(args, new[] { typeof(T) }, configureOptions);

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static void Run(Type[] commandTypes, Action<WildCocoLiteAppOptions>? configureOptions = null)
        => Run(null, commandTypes, configureOptions);

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    public static void Run(string[]? args, Type[] commandTypes, Action<WildCocoLiteAppOptions>? configureOptions = null)
    {
        var app = Create(args, configureOptions);
        app.AddCommands(commandTypes);

        app.Run();
    }

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync<T>(string[]? args = null, Action<WildCocoLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        => RunAsync(args, new[] { typeof(T) }, configureOptions, cancellationToken);

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync(Type[] commandTypes, Action<WildCocoLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        => RunAsync(null, commandTypes, configureOptions, cancellationToken);

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync(string[]? args, Type[] commandTypes, Action<WildCocoLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
    {
        var app = Create(args, configureOptions);
        app.AddCommands(commandTypes);

        return app.RunAsync(cancellationToken);
    }

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="commandBody"></param>
    /// <param name="configureCommand"></param>
    /// <param name="configureOptions"></param>
    public static void Run(Delegate commandBody, string[]? args = null, Action<CommandConventionBuilder>? configureCommand = null, Action<WildCocoLiteAppOptions>? configureOptions = null)
    {
        var app = Create(args, configureOptions);
        var commandConventionBuilder = app.AddCommand(commandBody);
        configureCommand?.Invoke(commandConventionBuilder);

        app.Run();
    }

    /// <summary>
    /// Starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="commandBody"></param>
    /// <param name="configureCommand"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync(Delegate commandBody, string[]? args = null, Action<CommandConventionBuilder>? configureCommand = null, Action<WildCocoLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
    {
        var app = Create(args, configureOptions);
        var commandConventionBuilder = app.AddCommand(commandBody);
        configureCommand?.Invoke(commandConventionBuilder);

        return app.RunAsync(cancellationToken);
    }
}
