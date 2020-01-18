using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cocona.Hosting
{
    public class CoconaAppHostBuilder
    {
        private readonly IHostBuilder _builder;

        public CoconaAppHostBuilder(IHostBuilder hostBuilder)
        {
            _builder = hostBuilder;
        }

        public CoconaAppHostBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
        {
            _builder.ConfigureServices(configureDelegate);
            return this;
        }

        public CoconaAppHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            _builder.ConfigureAppConfiguration(configureDelegate);
            return this;
        }

        private IHost GetBuiltHost(string[] args, Type[] types, Action<CoconaAppOptions>? configureOptions)
        {
            return _builder
                .UseCocona(args, types)
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

        public void Run<T>(string[] args, Action<CoconaAppOptions>? configureOptions = null)
            => Run(args, new[] { typeof(T) }, configureOptions);

        public Task RunAsync<T>(string[] args, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => RunAsync(args, new[] { typeof(T) }, configureOptions, cancellationToken);

        public void Run(string[] args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null)
            => GetBuiltHost(args, commandTypes, configureOptions).Run();

        public Task RunAsync(string[] args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => GetBuiltHost(args, commandTypes, configureOptions).RunAsync(cancellationToken);
    }
}
