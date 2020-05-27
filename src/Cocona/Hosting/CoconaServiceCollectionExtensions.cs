using System.Linq;
using Cocona;
using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Binder.Validation;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher;
using Cocona.CommandLine;
using Cocona.Help;
#if COCONA_LITE
using Cocona.Lite;
using Cocona.Lite.Hosting;
#else
using Cocona.Hosting;
#endif
using Cocona.ShellCompletion;
using Cocona.ShellCompletion.Candidate;
using Cocona.ShellCompletion.Generators;
#if !COCONA_LITE
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
#endif

#if COCONA_LITE
using IServiceCollection = Cocona.Lite.ICoconaLiteServiceCollection;
namespace Cocona.Lite.Hosting
#else
namespace Microsoft.Extensions.Hosting
#endif
{
    public static class CoconaServiceCollectionExtensions
    {
        public static IServiceCollection AddCoconaCore(this IServiceCollection services, string[] args)
        {
#if COCONA_LITE
            services.AddSingleton<ICoconaInstanceActivator>(_ => new CoconaLiteInstanceActivator());
#else
            services.AddSingleton<ICoconaInstanceActivator>(_ => new CoconaInstanceActivator());
#endif

            services.AddSingleton<ICoconaCommandProvider>(sp =>
            {
#if COCONA_LITE
                var options = (CoconaLiteAppOptions)sp.GetService(typeof(CoconaLiteAppOptions));
#else
                var options = sp.GetService<IOptions<CoconaAppOptions>>().Value;
#endif

                return new CoconaBuiltInCommandProvider(
                    new CoconaCommandProvider(
                        options.CommandTypes.ToArray(),
                        options.TreatPublicMethodsAsCommands,
                        options.EnableConvertOptionNameToLowerCase,
                        options.EnableConvertCommandNameToLowerCase
                    ),
                    options.EnableShellCompletionSupport
                );
            });
            services.TryAddSingleton<ICoconaCommandLineArgumentProvider>(serviceProvider =>
                new CoconaCommandLineArgumentProvider(args));
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

            return services;
        }

        public static IServiceCollection AddCoconaShellCompletion(this IServiceCollection services)
        {
            services.AddSingleton<ICoconaShellCompletionCodeGenerator, BashCoconaShellCompletionCodeGenerator>();
            services.AddSingleton<ICoconaShellCompletionCodeGenerator, ZshCoconaShellCompletionCodeGenerator>();
            services.TryAddSingleton<ICoconaShellCompletionCodeProvider, CoconaShellCompletionCodeProvider>();
            services.TryAddSingleton<ICoconaCompletionCandidatesMetadataFactory, CoconaCompletionCandidatesMetadataFactory>();
            services.TryAddSingleton<ICoconaCompletionCandidatesProviderFactory, CoconaCompletionCandidatesProviderFactory>();
            services.TryAddSingleton<ICoconaCompletionCandidates, CoconaCompletionCandidates>();

            return services;
        }
    }
}
