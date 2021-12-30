using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Builder;
using Cocona.Lite.Builder.Internal;
using Cocona.Lite.Hosting;

namespace Cocona.Lite.Builder
{
    /// <summary>
    /// A builder for console applications.
    /// </summary>
    public class CoconaLiteAppBuilder
    {
        private readonly CoconaLiteAppHostBuilder _hostBuilder;
        private readonly ICoconaLiteServiceCollection _services;
        private CoconaLiteApp? _application;

        public ICoconaLiteServiceCollection Services => _services;

        internal CoconaLiteAppBuilder(string[]? args, Action<CoconaLiteAppOptions>? configure)
        {
            _services = new CoconaLiteServiceCollection();

            _hostBuilder = new CoconaLiteAppHostBuilder(args);
            _hostBuilder.ConfigureOptions(configure);
            _hostBuilder.ConfigureApplication(app =>
            {
                // Copy commands from CoconaApp to CoconaAppHostOptions on starting application.
                foreach (var commandData in ((ICoconaCommandsBuilder)_application!).Build())
                {
                    app.AddCommand(commandData);
                }
            });
        }

        public CoconaLiteApp Build()
        {
            _hostBuilder.ConfigureServices((services) =>
            {
                foreach (var service in _services)
                {
                    services.Add(service);
                }
            });

            _application = new CoconaLiteApp(_hostBuilder.Build());
            return _application;
        }
    }
}
