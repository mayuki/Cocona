using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher;
using Cocona.CommandLine;
using Cocona.Help;
using Cocona.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocona.Command.Binder.Validation;
using Cocona.ShellCompletion;
using Cocona.ShellCompletion.Candidate;

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
                    services.TryAddSingleton<ICoconaInstanceActivator, CoconaInstanceActivator>();
                    services.TryAddSingleton<ICoconaCommandLineArgumentProvider>(serviceProvider =>
                        new CoconaCommandLineArgumentProvider(args));
                    services.TryAddSingleton<ICoconaCommandProvider>(serviceProvider =>
                    {
                        var options = serviceProvider.GetService<IOptions<CoconaAppOptions>>().Value;
                        return new CoconaBuiltInCommandProvider(
                            new CoconaCommandProvider(
                                options.CommandTypes.ToArray(),
                                options.TreatPublicMethodsAsCommands,
                                options.EnableConvertOptionNameToLowerCase,
                                options.EnableConvertCommandNameToLowerCase
                            )
                        );
                    });
                    services.TryAddSingleton<ICoconaCommandDispatcherPipelineBuilder, CoconaCommandDispatcherPipelineBuilder>();
                    services.TryAddSingleton<ICoconaAppContextAccessor, CoconaAppContextAccessor>();
                    services.TryAddSingleton<ICoconaApplicationMetadataProvider, CoconaApplicationMetadataProvider>();
                    services.TryAddSingleton<ICoconaConsoleProvider, CoconaConsoleProvider>();
                    services.TryAddSingleton<ICoconaParameterValidatorProvider, DataAnnotationsParameterValidatorProvider>();

                    services.TryAddTransient<ICoconaParameterBinder, CoconaParameterBinder>();
                    services.TryAddTransient<ICoconaValueConverter, CoconaValueConverter>();
                    services.TryAddTransient<ICoconaCommandLineParser, CoconaCommandLineParser>();
                    services.TryAddTransient<ICoconaCommandDispatcher, CoconaCommandDispatcher>();
                    services.TryAddTransient<ICoconaCommandMatcher, CoconaCommandMatcher>();
                    services.TryAddTransient<ICoconaCommandResolver, CoconaCommandResolver>();
                    services.TryAddTransient<ICoconaHelpRenderer, CoconaHelpRenderer>();
                    services.TryAddTransient<ICoconaCommandHelpProvider, CoconaCommandHelpProvider>();

                    services.AddSingleton<ICoconaShellCompletionCodeProvider, BashCoconaShellCompletionCodeProvider>();
                    services.AddSingleton<ICoconaShellCompletionCodeProvider, ZshCoconaShellCompletionCodeProvider>();
                    services.TryAddSingleton<ICoconaShellCompletionCodeGenerator, CoconaShellCompletionCodeGenerator>();
                    services.TryAddSingleton<ICoconaCompletionCandidatesMetadataFactory, CoconaCompletionCandidatesMetadataFactory>();
                    services.TryAddSingleton<ICoconaCompletionCandidatesProviderFactory, CoconaCompletionCandidatesProviderFactory>();
                    services.TryAddSingleton<ICoconaCompletionCandidates, CoconaCompletionCandidates>();

                    services.AddHostedService<CoconaHostedService>();

                    services.Configure<CoconaAppOptions>(options =>
                    {
                        options.CommandTypes = types;
                    });
                });
        }
    }
}
