using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Builder;
using Cocona.Lite.Builder;
using Cocona.Lite.Hosting;

namespace Cocona
{
    public partial class CoconaLiteApp
    {
        /// <summary>
        /// Creates an instance of <see cref="CoconaLiteAppHostBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public static CoconaLiteApp Create(string[]? args = null, Action<CoconaLiteAppOptions>? configureOptions = null)
            => CreateBuilder(args, configureOptions).Build();

        /// <summary>
        /// Creates an instance of <see cref="CoconaLiteAppBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public static CoconaLiteAppBuilder CreateBuilder(string[]? args = null, Action<CoconaLiteAppOptions>? configureOptions = null)
            => new CoconaLiteAppBuilder(args, configureOptions);

        /// <summary>
        /// Creates an instance of <see cref="CoconaLiteAppHostBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public static CoconaLiteAppHostBuilder CreateHostBuilder()
            => new CoconaLiteAppHostBuilder(Array.Empty<string>());

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static void Run<T>(string[]? args = null, Action<CoconaLiteAppOptions>? configureOptions = null)
            => Run(args, new[] { typeof(T) }, configureOptions);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static void Run(Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions = null)
            => Run(null, commandTypes, configureOptions);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        public static void Run(string[]? args, Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions = null)
        {
            var app = Create(args, configureOptions);
            app.AddCommands(commandTypes);

            app.Run();
        }

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync<T>(string[]? args = null, Action<CoconaLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => RunAsync(args, new[] { typeof(T) }, configureOptions, cancellationToken);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static Task RunAsync(Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => RunAsync(null, commandTypes, configureOptions, cancellationToken);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync(string[]? args, Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        {
            var app = Create(args, configureOptions);
            app.AddCommands(commandTypes);

            return app.RunAsync(cancellationToken);
        }

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandBody"></param>
        /// <param name="configureCommand"></param>
        /// <param name="configureOptions"></param>
        public static void Run(Delegate commandBody, string[]? args = null, Action<CommandConventionBuilder>? configureCommand = null, Action<CoconaLiteAppOptions>? configureOptions = null)
        {
            var app = Create(args, configureOptions);
            var commandConventionBuilder = app.AddCommand(commandBody);
            configureCommand?.Invoke(commandConventionBuilder);

            app.Run();
        }

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandBody"></param>
        /// <param name="configureCommand"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync(Delegate commandBody, string[]? args = null, Action<CommandConventionBuilder>? configureCommand = null, Action<CoconaLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        {
            var app = Create(args, configureOptions);
            var commandConventionBuilder = app.AddCommand(commandBody);
            configureCommand?.Invoke(commandConventionBuilder);

            return app.RunAsync(cancellationToken);
        }
    }
}
