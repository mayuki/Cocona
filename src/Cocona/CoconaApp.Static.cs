using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Builder;
using Cocona.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cocona
{
    public partial class CoconaApp
    {
        /// <summary>
        /// Creates an instance of <see cref="CoconaAppHostBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public static CoconaApp Create(string[]? args = default)
            => new CoconaAppBuilder(args).Build();

        /// <summary>
        /// Creates an instance of <see cref="CoconaAppBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public static CoconaAppBuilder CreateBuilder(string[]? args = default)
            => new CoconaAppBuilder(args);

        /// <summary>
        /// Creates an instance of <see cref="CoconaAppHostBuilder"/>.
        /// </summary>
        /// <returns></returns>
        public static CoconaAppHostBuilder CreateHostBuilder()
            => new CoconaAppHostBuilder(Host.CreateDefaultBuilder(null));

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static void Run<T>(string[]? args = null, Action<CoconaAppOptions>? configureOptions = null)
            => Run(args, new[] { typeof(T) }, configureOptions);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync<T>(string[]? args = null, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => RunAsync(args, new[] { typeof(T) }, configureOptions, cancellationToken);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        public static void Run(Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null)
            => Run(null, commandTypes, configureOptions);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        public static void Run(string[]? args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null)
        {
            var builder = CreateBuilder(args);
            if (configureOptions is not null)
            {
                builder.Services.Configure<CoconaAppOptions>(configureOptions);
            }

            var app = builder.Build();
            app.AddCommands(commandTypes);

            app.Run();
        }

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        public static Task RunAsync(Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null)
            => RunAsync(null, commandTypes, configureOptions);

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync(string[]? args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        {
            var builder = CreateBuilder(args);
            if (configureOptions is not null)
            {
                builder.Services.Configure<CoconaAppOptions>(configureOptions);
            }

            var app = builder.Build();
            app.AddCommands(commandTypes);

            return app.RunAsync(cancellationToken);
        }

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="commandBody"></param>
        /// <param name="args"></param>
        /// <param name="configureCommand"></param>
        /// <param name="configureOptions"></param>
        public static void Run(Delegate commandBody, string[]? args = null, Action<CommandConventionBuilder>? configureCommand = null, Action<CoconaAppOptions>? configureOptions = null)
        {
            var builder = CreateBuilder(args);
            if (configureOptions is not null)
            {
                builder.Services.Configure<CoconaAppOptions>(configureOptions);
            }

            var app = builder.Build();
            var commandConventionBuilder = app.AddCommand(commandBody);
            configureCommand?.Invoke(commandConventionBuilder);

            app.Run();
        }

        /// <summary>
        /// Starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="commandBody"></param>
        /// <param name="args"></param>
        /// <param name="configureCommand"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task RunAsync(Delegate commandBody, string[]? args = null, Action<CommandConventionBuilder>? configureCommand = null, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
        {
            var builder = CreateBuilder(args);
            if (configureOptions is not null)
            {
                builder.Services.Configure<CoconaAppOptions>(configureOptions);
            }

            var app = builder.Build();
            var commandConventionBuilder = app.AddCommand(commandBody);
            configureCommand?.Invoke(commandConventionBuilder);

            return app.RunAsync(cancellationToken);
        }
    }
}
