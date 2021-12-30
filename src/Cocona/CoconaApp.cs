using Cocona.Builder;
using Cocona.Builder.Internal;
using Cocona.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cocona
{
    /// <summary>
    /// Initializes and starts a Cocona enabled application.
    /// </summary>
    public partial class CoconaApp : ICoconaCommandsBuilder, ICoconaAppBuilder
    {
        private readonly IHost _host;
        private readonly ICoconaCommandsBuilder _commandsBuilder = new CoconaCommandsBuilder();

        public IServiceProvider Services => _host.Services;
        public IConfiguration Configuration => _host.Services.GetRequiredService<IConfiguration>();
        public IHostEnvironment Environment => _host.Services.GetRequiredService<IHostEnvironment>();
        public IHostApplicationLifetime Lifetime => _host.Services.GetRequiredService<IHostApplicationLifetime>();
        public ILogger Logger => _host.Services.GetRequiredService<ILogger<CoconaApp>>();

        #region Implements ICoconaCommandsBuilder
        List<ICommandDataSource> ICoconaCommandsBuilder.CommandDataSources => _commandsBuilder.CommandDataSources;
        ICoconaCommandsBuilder ICoconaCommandsBuilder.New() => _commandsBuilder.New();
        IReadOnlyList<ICommandData> ICoconaCommandsBuilder.Build() => _commandsBuilder.Build();
        #endregion

        public CoconaApp(IHost host)
        {
            _host = host;
        }

        public void Run()
            => _host.Run();

        public Task RunAsync(CancellationToken cancellationToken = default)
            => _host.RunAsync(cancellationToken);
    }
}
