using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cocona.Builder.Internal;
using Cocona.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

namespace Cocona.Builder
{
    /// <summary>
    /// A builder for console applications.
    /// </summary>
    public class CoconaAppBuilder
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly HostBuilder _hostBuilder = new HostBuilder();
        private readonly ConfigureHostBuilder _configureHostBuilder;
        private readonly Action<CoconaAppOptions>? _configureOptions;
        private CoconaApp? _application;

        public IServiceCollection Services => _services;
        public ILoggingBuilder Logging { get; }
        public IHostBuilder Host => _configureHostBuilder;
        public IHostEnvironment Environment { get; }
        public ConfigurationManager Configuration { get; }

        internal CoconaAppBuilder(string[]? args, Action<CoconaAppOptions>? configureOptions = null)
        {
            Configuration = new ConfigurationManager();
            _services = new ServiceCollection();
            _configureOptions = configureOptions;

            // Sets the default configuration values for the application host.
            // such as EnvironmentName, ApplicationName, ContentRoot...
            var bootstrapHostBuilder = new BootstrapHostBuilder(_services);
            bootstrapHostBuilder.ConfigureDefaultCocona(args, app =>
            {
                // Copy commands from CoconaApp to CoconaAppHostOptions on starting application.
                foreach (var commandData in ((ICoconaCommandsBuilder)_application!).Build())
                {
                    app.AddCommand(commandData);
                }
            });
            var (hostBuilderContext, hostConfiguration) = bootstrapHostBuilder.Apply(Configuration, _hostBuilder);

            _configureHostBuilder = new ConfigureHostBuilder(hostBuilderContext, Configuration, Services);
            Environment = new HostingEnvironment()
            {
                ApplicationName = hostBuilderContext.HostingEnvironment.ApplicationName ?? Assembly.GetEntryAssembly()!.GetName().Name,
                EnvironmentName = hostBuilderContext.HostingEnvironment.EnvironmentName,
                ContentRootFileProvider = hostBuilderContext.HostingEnvironment.ContentRootFileProvider,
                ContentRootPath = hostBuilderContext.HostingEnvironment.ContentRootPath,
            };
            Logging = new LoggingBuilder(_services);

            _services.AddSingleton<IConfiguration>(sp => Configuration);
        }


        public CoconaApp Build()
        {
            _hostBuilder.ConfigureAppConfiguration((hostBuilder, configuration) =>
            {
                var chainedSource = new ChainedConfigurationSource()
                {
                    Configuration = Configuration,
                    ShouldDisposeConfiguration = true,
                };
                configuration.Add(chainedSource);

                foreach (var keyValue in ((IConfigurationBuilder)Configuration).Properties)
                {
                    configuration.Properties[keyValue.Key] = keyValue.Value;
                }
            });

            _hostBuilder.ConfigureServices((hostBuilder, services) =>
            {
                services.AddSingleton(Environment);

                if (_configureOptions != null)
                {
                    services.Configure<CoconaAppOptions>(_configureOptions);
                }

                foreach (var service in _services.Where(x => !typeof(IHostedService).IsAssignableFrom(x.ServiceType)))
                {
                    services.Add(service);
                }
                foreach (var service in _services.Where(x => typeof(IHostedService).IsAssignableFrom(x.ServiceType)))
                {
                    services.Add(service);
                }
            });

            _configureHostBuilder.RunOperations(_hostBuilder);

            _application = new CoconaApp(_hostBuilder.Build());
            return _application;
        }
    }

    internal static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureDefaultCocona(this IHostBuilder hostBuilder, string[]? args, Action<ICoconaCommandsBuilder> configureApplication)
        {
            var builder = new CoconaAppHostBuilder(hostBuilder);
            builder.ConfigureDefaults(args, configureApplication);

            return hostBuilder;
        }
    }
}
