#if WildCoco_LITE
using WildCocoAppHostBuilder = WildCoco.Lite.Hosting.WildCocoLiteAppHostBuilder;
using WildCocoAppOptions = WildCoco.WildCocoLiteAppOptions;
using WildCoco.Lite.Hosting;
#else
using WildCoco.Hosting;
#endif

namespace WildCoco;

public static class WildCocoAppHostBuilderExtensions
{
    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="args"></param>
    /// <param name="configureOptions"></param>
    public static void Run<T>(this WildCocoAppHostBuilder builder, string[] args, Action<WildCocoAppOptions>? configureOptions = null)
        => builder.Run(args, new[] { typeof(T) }, configureOptions);

    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="args"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync(this WildCocoAppHostBuilder builder, string[] args, Action<WildCocoAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        => builder.RunAsync(args, Array.Empty<Type>(), configureOptions, cancellationToken);

    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="args"></param>
    /// <param name="configureOptions"></param>
    public static void Run(this WildCocoAppHostBuilder builder, string[] args, Action<WildCocoAppOptions>? configureOptions = null)
        => builder.Run(args, Array.Empty<Type>(), configureOptions);

    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="args"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync<T>(this WildCocoAppHostBuilder builder, string[] args, Action<WildCocoAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        => builder.RunAsync(args, new[] { typeof(T) }, configureOptions, cancellationToken);

    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    public static void Run<T>(this WildCocoAppHostBuilder builder, Action<WildCocoAppOptions>? configureOptions = null)
        => builder.Run(null, new[] { typeof(T) }, configureOptions);

    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync<T>(this WildCocoAppHostBuilder builder, Action<WildCocoAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        => builder.RunAsync(null, new[] { typeof(T) }, configureOptions, cancellationToken);

    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    public static void Run(this WildCocoAppHostBuilder builder, Action<WildCocoAppOptions>? configureOptions = null)
        => builder.Run(null, Array.Empty<Type>(), configureOptions);

    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync(this WildCocoAppHostBuilder builder, Action<WildCocoAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        => builder.RunAsync(null, Array.Empty<Type>(), configureOptions, cancellationToken);
}

#if WildCoco_LITE
public static class WildCocoLiteAppHostBuilderExtensions
{
    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="args"></param>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    public static void Run(this WildCocoAppHostBuilder builder, string[]? args, Type[] commandTypes, Action<WildCocoAppOptions>? configureOptions = null)
    {
        builder.ConfigureArguments(args);
        builder.ConfigureCommandTypes(commandTypes);
        builder.ConfigureOptions(configureOptions);

        builder.Build().RunAsync(default).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Builds host and starts the WildCoco enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="args"></param>
    /// <param name="commandTypes"></param>
    /// <param name="configureOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAsync(this WildCocoAppHostBuilder builder, string[]? args, Type[] commandTypes, Action<WildCocoAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
    {
        builder.ConfigureArguments(args);
        builder.ConfigureCommandTypes(commandTypes);
        builder.ConfigureOptions(configureOptions);

        return builder.Build().RunAsync(cancellationToken);
    }
}
#endif
