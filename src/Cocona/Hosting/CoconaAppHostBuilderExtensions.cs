using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        /// Add the commands type to Cocona.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static CoconaAppHostBuilder AddCommand<T>(this CoconaAppHostBuilder builder)
            => builder.AddCommand(typeof(T));

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
            => builder.Run(GetCommandLineArguments(), new[] { typeof(T) }, configureOptions);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync<T>(this CoconaAppHostBuilder builder, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => builder.RunAsync(GetCommandLineArguments(), new[] { typeof(T) }, configureOptions, cancellationToken);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        public static void Run(this CoconaAppHostBuilder builder, Action<CoconaAppOptions>? configureOptions = null)
            => builder.Run(GetCommandLineArguments(), Array.Empty<Type>(), configureOptions);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync(this CoconaAppHostBuilder builder, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => builder.RunAsync(GetCommandLineArguments(), Array.Empty<Type>(), configureOptions, cancellationToken);

        private static string[] GetCommandLineArguments()
        {
            var args = Environment.GetCommandLineArgs();
            return args.Any()
                ? args.Skip(1).ToArray() // args[0] is the path to executable binary.
                : Array.Empty<string>();
        }
    }
}
