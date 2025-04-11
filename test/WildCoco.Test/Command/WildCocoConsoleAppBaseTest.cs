using WildCoco.Application;
using WildCoco.Command;
using WildCoco.Command.Binder;
using WildCoco.Command.Binder.Validation;
using WildCoco.Command.Dispatcher;
using WildCoco.Command.Dispatcher.Middlewares;
using WildCoco.CommandLine;
using WildCoco.Application;
using WildCoco.Command;
using WildCoco.Command.Binder;
using WildCoco.Command.Dispatcher;
using WildCoco.Command.Dispatcher.Middlewares;
using WildCoco.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WildCoco.Command.Binder.Validation;
using WildCoco.Hosting;

namespace WildCoco.Test.Command;

public class WildCocoConsoleAppBaseTest
{
    private ServiceCollection CreateDefaultServices<TCommand>(string[] args)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IWildCocoParameterValidatorProvider, DataAnnotationsParameterValidatorProvider>();
        services.AddSingleton<IWildCocoCommandProvider>(serviceProvider => new WildCocoCommandProvider(new Type[] { typeof(TCommand) }));
        services.AddSingleton<IWildCocoCommandLineArgumentProvider>(serviceProvider => new WildCocoCommandLineArgumentProvider(args));
        services.AddTransient<IWildCocoParameterBinder, WildCocoParameterBinder>();
        services.AddTransient<IWildCocoValueConverter, WildCocoValueConverter>();
        services.AddTransient<IWildCocoCommandLineParser, WildCocoCommandLineParser>();
        services.AddTransient<IWildCocoCommandDispatcher, WildCocoCommandDispatcher>();
        services.AddTransient<IWildCocoCommandMatcher, WildCocoCommandMatcher>();
        services.AddTransient<IWildCocoCommandResolver, WildCocoCommandResolver>();
        services.AddSingleton<IWildCocoAppContextAccessor, WildCocoAppContextAccessor>();
        services.AddSingleton<IWildCocoInstanceActivator, WildCocoInstanceActivator>();
        services.AddSingleton<IWildCocoServiceProviderScopeSupport, WildCocoServiceProviderScopeSupport>();
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddSingleton<IWildCocoCommandDispatcherPipelineBuilder>(serviceProvider =>
            new WildCocoCommandDispatcherPipelineBuilder(serviceProvider, serviceProvider.GetService<IWildCocoInstanceActivator>())
                .UseMiddleware<InitializeConsoleAppMiddleware>()
                .UseMiddleware<WildCocoCommandInvokeMiddleware>()
        );

        // register a test command as singleton.
        services.AddSingleton<ConsoleAppTest>();

        return services;
    }

    [Fact]
    public async Task InitializeConsoleApp()
    {
        var services = CreateDefaultServices<ConsoleAppTest>(Array.Empty<string>());
        var serviceProvider = services.BuildServiceProvider();

        var commandInstance = serviceProvider.GetService<ConsoleAppTest>();
        var resolvedCommand = serviceProvider.GetRequiredService<IWildCocoCommandResolver>().ParseAndResolve(
            serviceProvider.GetRequiredService<IWildCocoCommandProvider>().GetCommandCollection(),
            serviceProvider.GetRequiredService<IWildCocoCommandLineArgumentProvider>().GetArguments()
        );
        var result = await serviceProvider.GetService<IWildCocoCommandDispatcher>().DispatchAsync(resolvedCommand);
        result.Should().Be(0);
        commandInstance.HasContext.Should().BeTrue();
    }

    class ConsoleAppTest : WildCocoConsoleAppBase
    {
        public bool HasContext { get; set; }

        public void Test()
        {
            HasContext = Context != null;
        }
    }
}
