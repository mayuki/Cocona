using Cocona.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cocona
{
    /// <summary>
    /// Initializes and starts a Cocona enabled application.
    /// </summary>
    public partial class CoconaApp : ICoconaCommandsBuilder, ICoconaAppBuilder, IDisposable
    {
        private readonly IHost _host;
        private readonly ICoconaCommandsBuilder _commandsBuilder = new CoconaCommandsBuilder();

        public IServiceProvider Services => _host.Services;
        public IConfiguration Configuration => _host.Services.GetRequiredService<IConfiguration>();
        public IHostEnvironment Environment => _host.Services.GetRequiredService<IHostEnvironment>();
        public IHostApplicationLifetime Lifetime => _host.Services.GetRequiredService<IHostApplicationLifetime>();
        public ILogger Logger => _host.Services.GetRequiredService<ILogger<CoconaApp>>();

        #region Implements ICoconaCommandsBuilder
        IDictionary<string, object?> ICoconaCommandsBuilder.Properties => _commandsBuilder.Properties;
        ICoconaCommandsBuilder ICoconaCommandsBuilder.New() => _commandsBuilder.New();
        IReadOnlyList<ICommandData> ICoconaCommandsBuilder.Build() => _commandsBuilder.Build();
        ICoconaCommandsBuilder ICoconaCommandsBuilder.Add(ICommandDataSource commandDataSource) => _commandsBuilder.Add(commandDataSource);
        #endregion

        public CoconaApp(IHost host)
        {
            _host = host;
        }

        public void Run()
            => _host.Run();

        public Task RunAsync(CancellationToken cancellationToken = default)
            => _host.RunAsync(cancellationToken);

        public void Dispose()
            => _host.Dispose();
    }
}
