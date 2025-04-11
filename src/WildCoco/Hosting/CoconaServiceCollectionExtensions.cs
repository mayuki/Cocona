using WildCoco;
using WildCoco.Application;



using Microsoft.Extensions.DependencyInjection;
using WildCoco.Builder;
using WildCoco.Command;
using WildCoco.Command.Binder;
using WildCoco.Command.Binder.Validation;
using WildCoco.Command.BuiltIn;
using WildCoco.Command.Dispatcher;
using WildCoco.CommandLine;
using WildCoco.Help;
using WildCoco.Lite;
using WildCoco.Lite.Builder.Internal;
using WildCoco.Localization.Internal;
using WildCoco.ShellCompletion;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Generators;
using WildCoco;
using WildCoco.Application;
using WildCoco.Builder;
using WildCoco.Command;
using WildCoco.Command.Binder;
using WildCoco.Command.Binder.Validation;
using WildCoco.Command.BuiltIn;
using WildCoco.Command.Dispatcher;
using WildCoco.CommandLine;
using WildCoco.Help;
using WildCoco.Localization.Internal;
#if WildCoco_LITE
using WildCoco.Lite;
using WildCoco.Lite.Builder.Internal;
using WildCoco.Lite.Hosting;
#else
using WildCoco.Builder.Internal;
using WildCoco.Hosting;
#endif
using WildCoco.ShellCompletion;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Generators;
#if !WildCoco_LITE
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
#endif

#if WildCoco_LITE
using IServiceCollection = WildCoco.Lite.IWildCocoLiteServiceCollection;

namespace WildCoco.Lite.Hosting;
#else
namespace Microsoft.Extensions.Hosting;
#endif

public static class WildCocoServiceCollectionExtensions
{
    internal static IServiceCollection AddWildCocoCore(this IServiceCollection services, string[]? args)
    {
#if WildCoco_LITE
        services.AddSingleton<IWildCocoInstanceActivator>(_ => new WildCocoLiteInstanceActivator());
        services.AddSingleton<IWildCocoServiceProviderIsService>(sp => sp.GetRequiredService<IWildCocoServiceProviderIsService>());
        services.AddSingleton<IWildCocoServiceProviderScopeSupport>(sp => sp.GetRequiredService<IWildCocoServiceProviderScopeSupport>());
#else
        services.AddSingleton<IWildCocoInstanceActivator>(_ => new WildCocoInstanceActivator());
        services.AddSingleton<IWildCocoServiceProviderIsService, WildCocoServiceProviderIsService>();
        services.AddSingleton<IWildCocoServiceProviderScopeSupport, WildCocoServiceProviderScopeSupport>();
#endif

        services.TryAddSingleton<IWildCocoCommandProvider>(sp =>
        {
#if WildCoco_LITE
            var options = sp.GetRequiredService<WildCocoLiteAppOptions>();
            var hostOptions = sp.GetRequiredService<WildCocoLiteAppHostOptions>();
#else
            var options = sp.GetRequiredService<IOptions<WildCocoAppOptions>>().Value;
            var hostOptions = sp.GetRequiredService<IOptions<WildCocoAppHostOptions>>().Value;
#endif
            var builder = new WildCocoCommandsBuilder();
            (hostOptions.ConfigureApplication ?? throw new InvalidOperationException("WildCocoAppHost is not initialized yet.")).Invoke(builder);

            var commandProcessOptions = CommandProviderOptions.None;
            commandProcessOptions |= options.TreatPublicMethodsAsCommands ? CommandProviderOptions.TreatPublicMethodAsCommands : CommandProviderOptions.None;
            commandProcessOptions |= options.EnableConvertOptionNameToLowerCase ? CommandProviderOptions.OptionNameToLowerCase : CommandProviderOptions.None;
            commandProcessOptions |= options.EnableConvertCommandNameToLowerCase ? CommandProviderOptions.CommandNameToLowerCase : CommandProviderOptions.None;
            commandProcessOptions |= options.EnableConvertArgumentNameToLowerCase ? CommandProviderOptions.ArgumentNameToLowerCase : CommandProviderOptions.None;

            return new WildCocoBuiltInCommandProvider(
                new WildCocoCommandProvider(builder.Build(), commandProcessOptions, sp.GetRequiredService<IWildCocoServiceProviderIsService>()),
                options.EnableShellCompletionSupport
            );
        });
        services.TryAddSingleton<IWildCocoEnvironmentProvider, DefaultWildCocoEnvironmentProvider>();
        services.TryAddSingleton<IWildCocoCommandLineArgumentProvider>(serviceProvider =>
            new WildCocoCommandLineArgumentProvider(args ?? serviceProvider.GetRequiredService<IWildCocoEnvironmentProvider>().GetCommandLineArgs()));
        services.TryAddSingleton<IWildCocoCommandDispatcherPipelineBuilder, WildCocoCommandDispatcherPipelineBuilder>();
        services.TryAddSingleton<IWildCocoAppContextAccessor, WildCocoAppContextAccessor>();
        services.TryAddSingleton<IWildCocoApplicationMetadataProvider, WildCocoApplicationMetadataProvider>();
        services.TryAddSingleton<IWildCocoConsoleProvider, WildCocoConsoleProvider>();
        services.TryAddSingleton<IWildCocoParameterValidatorProvider, DataAnnotationsParameterValidatorProvider>();
        services.TryAddSingleton<IWildCocoBootstrapper, WildCocoBootstrapper>();

        services.TryAddTransient<IWildCocoParameterBinder, WildCocoParameterBinder>();
        services.TryAddTransient<IWildCocoValueConverter, WildCocoValueConverter>();
        services.TryAddTransient<IWildCocoCommandLineParser, WildCocoCommandLineParser>();
        services.TryAddTransient<IWildCocoCommandDispatcher, WildCocoCommandDispatcher>();
        services.TryAddTransient<IWildCocoCommandMatcher, WildCocoCommandMatcher>();
        services.TryAddTransient<IWildCocoCommandResolver, WildCocoCommandResolver>();
        services.TryAddTransient<IWildCocoHelpRenderer, WildCocoHelpRenderer>();
        services.TryAddTransient<IWildCocoCommandHelpProvider, WildCocoCommandHelpProvider>();
        services.TryAddTransient<IWildCocoHelpMessageBuilder, WildCocoHelpMessageBuilder>();

        services.TryAddTransient<WildCocoLocalizerWrapper, WildCocoLocalizerWrapper>();

        services.TryAddTransient<WildCocoAppContext>(sp => sp.GetRequiredService<IWildCocoAppContextAccessor>().Current!);

        return services;
    }

    internal static IServiceCollection AddWildCocoShellCompletion(this IServiceCollection services)
    {
        services.AddSingleton<IWildCocoShellCompletionCodeGenerator, BashWildCocoShellCompletionCodeGenerator>();
        services.AddSingleton<IWildCocoShellCompletionCodeGenerator, ZshWildCocoShellCompletionCodeGenerator>();
        services.TryAddSingleton<IWildCocoShellCompletionCodeProvider, WildCocoShellCompletionCodeProvider>();
        services.TryAddSingleton<IWildCocoCompletionCandidatesMetadataFactory, WildCocoCompletionCandidatesMetadataFactory>();
        services.TryAddSingleton<IWildCocoCompletionCandidatesProviderFactory, WildCocoCompletionCandidatesProviderFactory>();
        services.TryAddSingleton<IWildCocoCompletionCandidates, WildCocoCompletionCandidates>();

        return services;
    }
}
