using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Binder.Validation;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher;
using Cocona.Command.Dispatcher.Middlewares;
using Cocona.CommandLine;
using Cocona.Help;
using Cocona.Lite;

namespace Cocona
{
    public class CoconaLiteAppHostBuilder
    {
        private Action<ICoconaLiteServiceCollection>? _configureServicesDelegate;

        /// <summary>
        /// Adds services to the container.
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        public CoconaLiteAppHostBuilder ConfigureServices(Action<ICoconaLiteServiceCollection> configureDelegate)
        {
            _configureServicesDelegate ??= _ => { };

            _configureServicesDelegate += configureDelegate;

            return this;
        }

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        public void Run<T>(string[] args, Action<CoconaLiteAppOptions>? configureOptions = null)
            => Run(args, new[] { typeof(T) }, configureOptions);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RunAsync<T>(string[] args, Action<CoconaLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => RunAsync(args, new[] { typeof(T) }, configureOptions, cancellationToken);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        public void Run(string[] args, Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions = null)
            => RunAsyncCore(args, commandTypes, configureOptions, default).GetAwaiter().GetResult();

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RunAsync(string[] args, Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => RunAsyncCore(args, commandTypes, configureOptions, cancellationToken);

        private async Task RunAsyncCore(string[] args, Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions, CancellationToken cancellationToken)
        {
            var services = new CoconaLiteServiceProvider();

            var options = new CoconaLiteAppOptions()
            {
                CommandTypes = commandTypes,
            };

            configureOptions?.Invoke(options);

            services.AddSingleton(options);

            services.AddSingleton<ICoconaInstanceActivator, CoconaLiteInstanceActivator>();
            services.AddSingleton<ICoconaCommandLineArgumentProvider>(serviceProvider =>
                new CoconaCommandLineArgumentProvider(args));
            services.AddSingleton<ICoconaCommandProvider>(serviceProvider =>
            {
                var options = (CoconaLiteAppOptions)serviceProvider.GetService(typeof(CoconaLiteAppOptions));

                return new CoconaBuiltInCommandProvider(
                    new CoconaCommandProvider(
                        options.CommandTypes.ToArray(),
                        options.TreatPublicMethodsAsCommands,
                        options.EnableConvertOptionNameToLowerCase,
                        options.EnableConvertCommandNameToLowerCase
                    )
                );
            });
            services.AddSingleton<ICoconaCommandDispatcherPipelineBuilder, CoconaCommandDispatcherPipelineBuilder>();
            services.AddSingleton<ICoconaApplicationMetadataProvider, CoconaApplicationMetadataProvider>();
            services.AddSingleton<ICoconaConsoleProvider, CoconaConsoleProvider>();
            services.AddSingleton<ICoconaParameterValidatorProvider, DataAnnotationsParameterValidatorProvider>();

            services.AddSingleton<ICoconaParameterBinder, CoconaParameterBinder>();
            services.AddSingleton<ICoconaValueConverter, CoconaValueConverter>();
            services.AddSingleton<ICoconaCommandLineParser, CoconaCommandLineParser>();
            services.AddSingleton<ICoconaCommandDispatcher, CoconaCommandDispatcher>();
            services.AddSingleton<ICoconaCommandMatcher, CoconaCommandMatcher>();
            services.AddSingleton<ICoconaHelpRenderer, CoconaHelpRenderer>();
            services.AddSingleton<ICoconaCommandHelpProvider, CoconaCommandHelpProvider>();

            IServiceProvider serviceProvider = services;
            serviceProvider.GetService<ICoconaCommandDispatcherPipelineBuilder>()
                .UseMiddleware<BuiltInCommandMiddleware>()
                .UseMiddleware<HandleExceptionAndExitMiddleware>()
                .UseMiddleware<HandleParameterBindExceptionMiddleware>()
                .UseMiddleware<RejectUnknownOptionsMiddleware>()
                .UseMiddleware<CommandFilterMiddleware>()
                .UseMiddleware<InitializeCoconaLiteConsoleAppMiddleware>()
                .UseMiddleware<CoconaCommandInvokeMiddleware>();

            _configureServicesDelegate?.Invoke(services);

            var commandDispatcher = serviceProvider.GetService<ICoconaCommandDispatcher>();

            Environment.ExitCode = await Task.Run(async () => await commandDispatcher.DispatchAsync(default));
        }
    }

    public class CoconaLiteApp
    {
        public static CoconaLiteAppHostBuilder Create()
            => new CoconaLiteAppHostBuilder();

        public static Task RunAsync<T>(string[] args, Action<CoconaLiteAppOptions>? configureOptions = default, CancellationToken cancellationToken = default)
        {
            return Create().RunAsync<T>(args, configureOptions, cancellationToken);
        }
    }
}
