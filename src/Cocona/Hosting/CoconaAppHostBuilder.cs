using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cocona.Hosting
{
    /// <summary>
    /// Builder for Cocona enabled application host.
    /// </summary>
    public class CoconaAppHostBuilder
    {
        private readonly IHostBuilder _builder;
        private readonly List<Delegate> _targetDelegates = new List<Delegate>();
        private readonly List<Type> _targetTypes = new List<Type>();

        public CoconaAppHostBuilder(IHostBuilder hostBuilder)
        {
            _builder = hostBuilder;
        }

        /// <summary>
        /// Adds services to the container. See also <seealso cref="HostingHostBuilderExtensions.ConfigureServices(IHostBuilder, Action{IServiceCollection})"/>.
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        public CoconaAppHostBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
        {
            _builder.ConfigureServices(configureDelegate);
            return this;
        }

        /// <summary>
        /// Adds services to the container. See also <seealso cref="HostingHostBuilderExtensions.ConfigureServices(IHostBuilder, Action{IServiceCollection})"/>.
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        public CoconaAppHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            _builder.ConfigureServices(configureDelegate);
            return this;
        }

        /// <summary>
        /// Sets up the configuration for the application. See also <seealso cref="HostingHostBuilderExtensions.ConfigureAppConfiguration(IHostBuilder, Action{IConfigurationBuilder})"/>.
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        public CoconaAppHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            _builder.ConfigureAppConfiguration(configureDelegate);
            return this;
        }

        /// <summary>
        /// Sets up the configuration for the application. See also <seealso cref="HostingHostBuilderExtensions.ConfigureAppConfiguration(IHostBuilder, Action{IConfigurationBuilder})"/>.
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        public CoconaAppHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            _builder.ConfigureAppConfiguration(configureDelegate);
            return this;
        }

        /// <summary>
        /// Adds a delegate for configuring the provided <see cref="ILoggingBuilder"/>. This may be called multiple times. See also <seealso cref="HostingHostBuilderExtensions.ConfigureLogging(Microsoft.Extensions.Hosting.IHostBuilder,System.Action{Microsoft.Extensions.Hosting.HostBuilderContext,Microsoft.Extensions.Logging.ILoggingBuilder})"/>.
        /// </summary>
        /// <param name="configureLogging"></param>
        /// <returns></returns>
        public CoconaAppHostBuilder ConfigureLogging(Action<ILoggingBuilder> configureLogging)
        {
            _builder.ConfigureLogging(configureLogging);
            return this;
        }

        private IHost GetBuiltHost(string[] args, Type[] types, Action<CoconaAppOptions>? configureOptions)
        {
            return _builder
                .UseCocona(args, _targetTypes.Concat(types), _targetDelegates)
                .ConfigureServices(services =>
                {
                    if (configureOptions != null)
                    {
                        services.Configure(configureOptions);
                    }
                })
                .UseConsoleLifetime(options => options.SuppressStatusMessages = true)
                .Build();
        }

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        public void Run(string[] args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null)
            => GetBuiltHost(args, commandTypes, configureOptions).Run();

        /// <summary>
        /// Builds host and starts the Cocona enabled application, and waits for Ctrl+C or SIGTERM to shutdown.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandTypes"></param>
        /// <param name="configureOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RunAsync(string[] args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => GetBuiltHost(args, commandTypes, configureOptions).RunAsync(cancellationToken);

        /// <summary>
        /// Add command definition delegate to Cocona.
        /// </summary>
        /// <param name="commandDelegate"></param>
        /// <returns></returns>
        public CoconaAppHostBuilder AddCommand(Delegate commandDelegate)
        {
            _targetDelegates.Add(commandDelegate ?? throw new ArgumentNullException(nameof(commandDelegate)));
            return this;
        }

        /// <summary>
        /// Add the commands type to Cocona.
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public CoconaAppHostBuilder AddCommand(Type commandType)
        {
            _targetTypes.Add(commandType ?? throw new ArgumentNullException(nameof(commandType)));
            return this;
        }
    }
}
