#if COCONA_LITE
using CoconaAppHostBuilder = Cocona.Lite.Hosting.CoconaLiteAppHostBuilder;
using CoconaAppOptions = Cocona.CoconaLiteAppOptions;
using Cocona.Lite.Hosting;
#else
using Cocona.Hosting;
#endif

namespace Cocona
{
    public static class CoconaAppHostBuilderExtensions
    {
        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        public static void Run<T>(this CoconaAppHostBuilder builder, string[] args, Action<CoconaAppOptions>? configureOptions = null)
            => builder.Run(args, new[] { typeof(T) }, configureOptions);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync(this CoconaAppHostBuilder builder, string[] args, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => builder.RunAsync(args, Array.Empty<Type>(), configureOptions, cancellationToken);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        public static void Run(this CoconaAppHostBuilder builder, string[] args, Action<CoconaAppOptions>? configureOptions = null)
            => builder.Run(args, Array.Empty<Type>(), configureOptions);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync<T>(this CoconaAppHostBuilder builder, string[] args, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => builder.RunAsync(args, new[] { typeof(T) }, configureOptions, cancellationToken);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        public static void Run<T>(this CoconaAppHostBuilder builder, Action<CoconaAppOptions>? configureOptions = null)
            => builder.Run(null, new[] { typeof(T) }, configureOptions);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync<T>(this CoconaAppHostBuilder builder, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => builder.RunAsync(null, new[] { typeof(T) }, configureOptions, cancellationToken);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        public static void Run(this CoconaAppHostBuilder builder, Action<CoconaAppOptions>? configureOptions = null)
            => builder.Run(null, Array.Empty<Type>(), configureOptions);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync(this CoconaAppHostBuilder builder, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => builder.RunAsync(null, Array.Empty<Type>(), configureOptions, cancellationToken);
    }

#if COCONA_LITE
    public static class CoconaLiteAppHostBuilderExtensions
    {
        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        public static void Run(this CoconaAppHostBuilder builder, string[]? args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null)
        {
            builder.ConfigureArguments(args);
            builder.ConfigureCommandTypes(commandTypes);
            builder.ConfigureOptions(configureOptions);

            builder.Build().RunAsync(default).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync(this CoconaAppHostBuilder builder, string[]? args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        {
            builder.ConfigureArguments(args);
            builder.ConfigureCommandTypes(commandTypes);
            builder.ConfigureOptions(configureOptions);

            return builder.Build().RunAsync(cancellationToken);
        }
    }
#endif
}
