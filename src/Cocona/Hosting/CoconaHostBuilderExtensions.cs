using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Dispatcher;
using Cocona.CommandLine;
using Cocona.Help;
using Cocona.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Hosting
{
    public static class CoconaHostBuilderExtensions
    {
        public static IHostBuilder UseCocona(this IHostBuilder hostBuilder, string[] args, Type[] types)
        {
            return hostBuilder
                .ConfigureLogging(logging =>
                {
                })
                .ConfigureAppConfiguration(config =>
                {
                })
                .ConfigureServices(services =>
                {
                    services.AddOptions<ConsoleLifetimeOptions>()
                        .Configure(x => x.SuppressStatusMessages = true);

                    services.TryAddSingleton<ICoconaCommandLineArgumentProvider>(serviceProvider => new CoconaCommandLineArgumentProvider(args));
                    services.TryAddSingleton<ICoconaCommandProvider>(serviceProvider => new CoconaCommandProvider(types));
                    services.TryAddSingleton<ICoconaCommandDispatcherPipelineBuilder, CoconaCommandDispatcherPipelineBuilder>();
                    services.TryAddSingleton<ICoconaAppContextAccessor, CoconaAppContextAccessor>();
                    services.TryAddSingleton<ICoconaApplicationMetadataProvider, CoconaApplicationMetadataProvider>();

                    services.TryAddTransient<ICoconaParameterBinder, CoconaParameterBinder>();
                    services.TryAddTransient<ICoconaValueConverter, CoconaValueConverter>();
                    services.TryAddTransient<ICoconaCommandLineParser, CoconaCommandLineParser>();
                    services.TryAddTransient<ICoconaCommandDispatcher, CoconaCommandDispatcher>();
                    services.TryAddTransient<ICoconaHelpRenderer, CoconaHelpRenderer>();
                    services.TryAddTransient<ICoconaCommandHelpProvider, CoconaCommandHelpProvider>();

                    services.AddHostedService<CoconaHostedService>();
                });
        }
    }
}
