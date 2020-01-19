using Cocona.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cocona
{
    /// <summary>
    /// Initializes and starts a Cocona enabled application.
    /// </summary>
    public class CoconaApp
    {
        /// <summary>
        /// Creates an instance of <see cref="CoconaAppHostBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public static CoconaAppHostBuilder Create()
            =>  new CoconaAppHostBuilder(Host.CreateDefaultBuilder(null /* If `arg` is null, CreateDefaultBuilder doesn't use `config.AddCommandLine()`. */));

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        public static void Run<T>(string[] args, Action<CoconaAppOptions>? configureOptions = null)
            => Run(args, new[] { typeof(T) }, configureOptions);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync<T>(string[] args, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => RunAsync(args, new[] { typeof(T) }, configureOptions, cancellationToken);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        public static void Run(string[] args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null)
            => Create().Run(args, commandTypes, configureOptions);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync(string[] args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => Create().RunAsync(args, commandTypes, configureOptions, cancellationToken);

    }
}
