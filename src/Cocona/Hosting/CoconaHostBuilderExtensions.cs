using Cocona.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocona.Builder.Internal;
using Cocona;
using Cocona.Builder;

namespace Microsoft.Extensions.Hosting
{
    public static class CoconaHostBuilderExtensions
    {
        public static IHostBuilder AddCocona(this IHostBuilder hostBuilder, string[]? args, IEnumerable<Type> types, IEnumerable<Delegate>? methods = null)
            => hostBuilder.AddCocona(args, app =>
                {
                    app.AddCommands(types);

                    foreach (var method in (methods ?? Array.Empty<Delegate>()))
                    {
                        app.AddCommand(method);
                    }
                });

        public static IHostBuilder AddCocona(this IHostBuilder hostBuilder, string[]? args, Action<ICoconaCommandsBuilder>? configureApplication = null)
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
                    services.AddCoconaCore(args ?? GetCommandLineArguments());
                    services.AddCoconaShellCompletion();

                    services.AddHostedService<CoconaHostedService>();

                    services.Configure<CoconaAppHostOptions>(options =>
                    {
                        options.ConfigureApplication = app =>
                        {
                            configureApplication?.Invoke(app);
                        };
                    });
                });
        }

        private static string[] GetCommandLineArguments()
        {
            var args = System.Environment.GetCommandLineArgs();
            return args.Any()
                ? args.Skip(1).ToArray() // args[0] is the path to executable binary.
                : Array.Empty<string>();
        }
    }
}
