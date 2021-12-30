using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Builder;
using Cocona.Lite.Builder;
using Cocona.Lite.Hosting;

namespace Cocona
{
    /// <summary>
    /// Initializes and starts a Cocona enabled application.
    /// </summary>
    public partial class CoconaLiteApp : ICoconaCommandsBuilder, ICoconaAppBuilder
    {
        private readonly ICoconaCommandsBuilder _builder;
        private readonly CoconaLiteAppHost _host;

        public CoconaLiteApp(CoconaLiteAppHost host)
        {
            _host = host;
            _builder = new CoconaCommandsBuilder();
        }

        List<ICommandDataSource> ICoconaCommandsBuilder.CommandDataSources
            => _builder.CommandDataSources;
        ICoconaCommandsBuilder ICoconaCommandsBuilder.New()
            => _builder.New();
        IReadOnlyList<ICommandData> ICoconaCommandsBuilder.Build()
            => _builder.Build();

        public Task RunAsync(CancellationToken cancellationToken = default)
            => _host.RunAsync(cancellationToken);
        public void Run()
            => _host.RunAsync(default).GetAwaiter().GetResult();
    }
}
