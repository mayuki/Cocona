using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cocona.Builder.Internal
{
    internal class ConfigureHostBuilder : IHostBuilder
    {
        private readonly List<Action<IHostBuilder>> _operations = new();

        private readonly HostBuilderContext _context;
        private readonly ConfigurationManager _configuration;
        private readonly IServiceCollection _services;

        public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

        public ConfigureHostBuilder(HostBuilderContext context, ConfigurationManager configuration, IServiceCollection services)
        {
            _context = context;
            _configuration = configuration;
            _services = services;
        }

        public IHost Build()
        {
            throw new InvalidOperationException();
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            configureDelegate(_context, _configuration);
            return this;
        }

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            _operations.Add(builder => builder.ConfigureContainer<TContainerBuilder>(configureDelegate));
            return this;
        }

        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            configureDelegate(_configuration);
            return this;
        }

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            configureDelegate(_context, _services);
            return this;
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
            where TContainerBuilder : notnull
        {
            _operations.Add(builder => builder.UseServiceProviderFactory<TContainerBuilder>(factory));
            return this;
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
            where TContainerBuilder : notnull
        {
            _operations.Add(builder => builder.UseServiceProviderFactory<TContainerBuilder>(factory));
            return this;
        }

        internal void RunOperations(IHostBuilder hostBuilder)
        {
            foreach (var action in _operations)
            {
                action(hostBuilder);
            }
        }
    }
}
