using Cocona.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocona.Application;
using Cocona.Command;
using Cocona.Command.BuiltIn;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Hosting
{
    public static class CoconaHostBuilderExtensions
    {
        public static IHostBuilder UseCocona(this IHostBuilder hostBuilder, string[] args, IEnumerable<Type> types, IEnumerable<Delegate>? methods = default)
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
                    services.AddCoconaCore(args);
                    services.AddCoconaShellCompletion();

                    services.AddHostedService<CoconaHostedService>();

                    services.Configure<CoconaAppOptions>(options =>
                    {
                        options.CommandTypes = types.ToList();
                        options.CommandMethods = (methods ?? Array.Empty<Delegate>()).ToList();
                    });
                });
        }
    }
}
