using System;
using System.Linq;
using System.Reflection;
using Cocona;
using Cocona.Application;
using Cocona.Builder;
using Cocona.Command;
using Cocona.Command.Binder;
using Cocona.Command.Binder.Validation;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher;
using Cocona.CommandLine;
using Cocona.Help;
using Cocona.Localization.Internal;
#if COCONA_LITE
using Cocona.Lite;
using Cocona.Lite.Builder.Internal;
using Cocona.Lite.Hosting;
#else
using Cocona.Builder.Internal;
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
        internal static IServiceCollection AddCoconaCore(this IServiceCollection services, string[]? args)
        {
#if COCONA_LITE
            services.AddSingleton<ICoconaInstanceActivator>(_ => new CoconaLiteInstanceActivator());
            services.AddSingleton<ICoconaServiceProviderIsService>(sp => sp.GetRequiredService<ICoconaServiceProviderIsService>());
            services.AddSingleton<ICoconaServiceProviderScopeSupport>(sp => sp.GetRequiredService<ICoconaServiceProviderScopeSupport>());
#else
            services.AddSingleton<ICoconaInstanceActivator>(_ => new CoconaInstanceActivator());
            services.AddSingleton<ICoconaServiceProviderIsService, CoconaServiceProviderIsService>();
            services.AddSingleton<ICoconaServiceProviderScopeSupport, CoconaServiceProviderScopeSupport>();
#endif

            services.TryAddSingleton<ICoconaCommandProvider>(sp =>
            {
#if COCONA_LITE
                var options = sp.GetRequiredService<CoconaLiteAppOptions>();
                var hostOptions = sp.GetRequiredService<CoconaLiteAppHostOptions>();
#else
                var options = sp.GetRequiredService<IOptions<CoconaAppOptions>>().Value;
                var hostOptions = sp.GetRequiredService<IOptions<CoconaAppHostOptions>>().Value;
#endif
                var builder = new CoconaCommandsBuilder();
                (hostOptions.ConfigureApplication ?? throw new InvalidOperationException("CoconaAppHost is not initialized yet.")).Invoke(builder);

                var commandProcessOptions = CommandProviderOptions.None;
                commandProcessOptions |= options.TreatPublicMethodsAsCommands ? CommandProviderOptions.TreatPublicMethodAsCommands : CommandProviderOptions.None;
                commandProcessOptions |= options.EnableConvertOptionNameToLowerCase ? CommandProviderOptions.OptionNameToLowerCase : CommandProviderOptions.None;
                commandProcessOptions |= options.EnableConvertCommandNameToLowerCase ? CommandProviderOptions.CommandNameToLowerCase : CommandProviderOptions.None;
                commandProcessOptions |= options.EnableConvertArgumentNameToLowerCase ? CommandProviderOptions.ArgumentNameToLowerCase : CommandProviderOptions.None;

                return new CoconaBuiltInCommandProvider(
                    new CoconaCommandProvider(builder.Build(), commandProcessOptions, sp.GetRequiredService<ICoconaServiceProviderIsService>()),
                    options.EnableShellCompletionSupport
                );
            });
            services.TryAddSingleton<ICoconaEnvironmentProvider, DefaultCoconaEnvironmentProvider>();
            services.TryAddSingleton<ICoconaCommandLineArgumentProvider>(serviceProvider =>
                new CoconaCommandLineArgumentProvider(args ?? serviceProvider.GetRequiredService<ICoconaEnvironmentProvider>().GetCommandLineArgs()));
            services.TryAddSingleton<ICoconaCommandDispatcherPipelineBuilder, CoconaCommandDispatcherPipelineBuilder>();
            services.TryAddSingleton<ICoconaAppContextAccessor, CoconaAppContextAccessor>();
            services.TryAddSingleton<ICoconaApplicationMetadataProvider, CoconaApplicationMetadataProvider>();
            services.TryAddSingleton<ICoconaConsoleProvider, CoconaConsoleProvider>();
            services.TryAddSingleton<ICoconaParameterValidatorProvider, DataAnnotationsParameterValidatorProvider>();
            services.TryAddSingleton<ICoconaBootstrapper, CoconaBootstrapper>();

            services.TryAddTransient<ICoconaParameterBinder, CoconaParameterBinder>();
            services.TryAddTransient<ICoconaValueConverter, CoconaValueConverter>();
            services.TryAddTransient<ICoconaCommandLineParser, CoconaCommandLineParser>();
            services.TryAddTransient<ICoconaCommandDispatcher, CoconaCommandDispatcher>();
            services.TryAddTransient<ICoconaCommandMatcher, CoconaCommandMatcher>();
            services.TryAddTransient<ICoconaCommandResolver, CoconaCommandResolver>();
            services.TryAddTransient<ICoconaHelpRenderer, CoconaHelpRenderer>();
            services.TryAddTransient<ICoconaCommandHelpProvider, CoconaCommandHelpProvider>();
            services.TryAddTransient<ICoconaHelpMessageBuilder, CoconaHelpMessageBuilder>();

            services.TryAddTransient<CoconaLocalizerWrapper, CoconaLocalizerWrapper>();

            services.TryAddTransient<CoconaAppContext>(sp => sp.GetRequiredService<ICoconaAppContextAccessor>().Current!);

            return services;
        }

        internal static IServiceCollection AddCoconaShellCompletion(this IServiceCollection services)
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
