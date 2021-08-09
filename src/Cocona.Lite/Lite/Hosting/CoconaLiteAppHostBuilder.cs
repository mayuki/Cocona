using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Application;
using Cocona.Command;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher;
using Cocona.Command.Dispatcher.Middlewares;

namespace Cocona.Lite.Hosting
{
    public class CoconaLiteAppHostBuilder
    {
        private Action<ICoconaLiteServiceCollection>? _configureServicesDelegate;
        private readonly List<Delegate> _targetDelegates = new List<Delegate>();
        private readonly List<Type> _targetTypes = new List<Type>();

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

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        public void Run(string[] args, Action<CoconaLiteAppOptions>? configureOptions = null)
            => new CoconaLiteAppHost(Build(args, Array.Empty<Type>(), configureOptions)).RunAsyncCore(default).GetAwaiter().GetResult();

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RunAsync(string[] args, Action<CoconaLiteAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => new CoconaLiteAppHost(Build(args, Array.Empty<Type>(), configureOptions)).RunAsyncCore(cancellationToken);

        /// <summary>
        /// Add command definition delegate to Cocona.
        /// </summary>
        /// <param name="commandDelegate"></param>
        /// <returns></returns>
        public CoconaLiteAppHostBuilder AddCommand(Delegate commandDelegate)
        {
            _targetDelegates.Add(commandDelegate ?? throw new ArgumentNullException(nameof(commandDelegate)));
            return this;
        }

        /// <summary>
        /// Add the commands type to Cocona.
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public CoconaLiteAppHostBuilder AddCommand(Type commandType)
        {
            _targetTypes.Add(commandType ?? throw new ArgumentNullException(nameof(commandType)));
            return this;
        }

        /// <summary>
        /// Add the commands type to Cocona.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CoconaLiteAppHostBuilder AddCommand<T>()
            => AddCommand(typeof(T));

        private IServiceProvider Build(string[] args, Type[] commandTypes, Action<CoconaLiteAppOptions>? configureOptions)
        {
            var services = new CoconaLiteServiceCollection();

            var options = new CoconaLiteAppOptions()
            {
                CommandTypes = _targetTypes.Concat(commandTypes).ToList(),
                CommandMethods = _targetDelegates.ToList(),
            };

            configureOptions?.Invoke(options);

            services.AddSingleton(options);

            services.AddCoconaCore(args);
            services.AddCoconaShellCompletion();

            _configureServicesDelegate?.Invoke(services);

            var serviceProvider = new CoconaLiteServiceProvider(services);
            serviceProvider.GetRequiredService<ICoconaCommandDispatcherPipelineBuilder>()
                .UseMiddleware<HandleExceptionAndExitMiddleware>()
                .UseMiddleware<HandleParameterBindExceptionMiddleware>()
                .UseMiddleware<RejectUnknownOptionsMiddleware>()
                .UseMiddleware<CommandFilterMiddleware>()
                .UseMiddleware((next, sp) => new InitializeCoconaLiteConsoleAppMiddleware(next, sp.GetRequiredService<ICoconaAppContextAccessor>()))
                .UseMiddleware<CoconaCommandInvokeMiddleware>();
            return serviceProvider;
        }
    }
}
