using System;
using System.Linq;
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

namespace Cocona.Lite.Hosting
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
            => Run(args, new[] {typeof(T)}, configureOptions);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RunAsync<T>(string[] args, Action<CoconaLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => RunAsync(args, new[] {typeof(T)}, configureOptions, cancellationToken);

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        public void Run(string[] args, Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions = null)
            => new CoconaLiteAppHost(Build(args, commandTypes, configureOptions)).RunAsyncCore(default).GetAwaiter().GetResult();

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RunAsync(string[] args, Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => new CoconaLiteAppHost(Build(args, commandTypes, configureOptions)).RunAsyncCore(cancellationToken);

        private IServiceProvider Build(string[] args, Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions)
        {
            var services = new CoconaLiteServiceProvider();

            var options = new CoconaLiteAppOptions()
            {
                CommandTypes = commandTypes,
            };

            configureOptions?.Invoke(options);

            services.AddSingleton(options);

            services.AddSingleton<ICoconaInstanceActivator>(_ => new CoconaLiteInstanceActivator());
            services.AddSingleton<ICoconaCommandLineArgumentProvider>(sp =>
                new CoconaCommandLineArgumentProvider(args));
            services.AddSingleton<ICoconaCommandProvider>(sp =>
            {
                var options = (CoconaLiteAppOptions)sp.GetService(typeof(CoconaLiteAppOptions));

                return new CoconaBuiltInCommandProvider(
                    new CoconaCommandProvider(
                        options.CommandTypes.ToArray(),
                        options.TreatPublicMethodsAsCommands,
                        options.EnableConvertOptionNameToLowerCase,
                        options.EnableConvertCommandNameToLowerCase
                    )
                );
            });
            services.AddSingleton<ICoconaCommandDispatcherPipelineBuilder>(sp => new CoconaCommandDispatcherPipelineBuilder(sp, sp.GetService<ICoconaInstanceActivator>()));
            services.AddSingleton<ICoconaAppContextAccessor>(sp => new CoconaAppContextAccessor());
            services.AddSingleton<ICoconaApplicationMetadataProvider>(sp => new CoconaApplicationMetadataProvider());
            services.AddSingleton<ICoconaConsoleProvider>(sp => new CoconaConsoleProvider());
            services.AddSingleton<ICoconaParameterValidatorProvider>(sp => new DataAnnotationsParameterValidatorProvider());

            services.AddSingleton<ICoconaParameterBinder>(sp => new CoconaParameterBinder(sp, sp.GetService<ICoconaValueConverter>(), sp.GetService<ICoconaParameterValidatorProvider>()));
            services.AddSingleton<ICoconaValueConverter>(sp => new CoconaValueConverter());
            services.AddSingleton<ICoconaCommandLineParser>(sp => new CoconaCommandLineParser());
            services.AddSingleton<ICoconaCommandDispatcher>(sp => new CoconaCommandDispatcher(sp, sp.GetService<ICoconaCommandProvider>(), sp.GetService<ICoconaCommandLineParser>(), sp.GetService<ICoconaCommandLineArgumentProvider>(), sp.GetService<ICoconaCommandDispatcherPipelineBuilder>(), sp.GetService<ICoconaCommandMatcher>(), sp.GetService<ICoconaInstanceActivator>(), sp.GetService<ICoconaAppContextAccessor>()));
            services.AddSingleton<ICoconaCommandMatcher>(sp => new CoconaCommandMatcher());
            services.AddSingleton<ICoconaHelpRenderer>(sp => new CoconaHelpRenderer());
            services.AddSingleton<ICoconaCommandHelpProvider>(sp => new CoconaCommandHelpProvider(sp.GetService<ICoconaApplicationMetadataProvider>(), sp));

            _configureServicesDelegate?.Invoke(services);

            IServiceProvider serviceProvider = services;
            serviceProvider.GetService<ICoconaCommandDispatcherPipelineBuilder>()
                .UseMiddleware<BuiltInCommandMiddleware>()
                .UseMiddleware<HandleExceptionAndExitMiddleware>()
                .UseMiddleware<HandleParameterBindExceptionMiddleware>()
                .UseMiddleware<RejectUnknownOptionsMiddleware>()
                .UseMiddleware<CommandFilterMiddleware>()
                .UseMiddleware((next, sp) => new InitializeCoconaLiteConsoleAppMiddleware(next, sp.GetService<ICoconaAppContextAccessor>()))
                .UseMiddleware<CoconaCommandInvokeMiddleware>();
            return serviceProvider;
        }
    }
}
