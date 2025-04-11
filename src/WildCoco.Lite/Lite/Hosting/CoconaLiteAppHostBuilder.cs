using WildCoco.Application;
using WildCoco.Builder;
using WildCoco.Command.Dispatcher;
using WildCoco.Command.Dispatcher.Middlewares;
using WildCoco.Lite.Builder.Internal;
using WildCoco.Application;
using WildCoco.Builder;
using WildCoco.Command.Dispatcher;
using WildCoco.Command.Dispatcher.Middlewares;
using WildCoco.Lite.Builder.Internal;

namespace WildCoco.Lite.Hosting;

public class WildCocoLiteAppHostBuilder
{
    private Action<IWildCocoLiteServiceCollection>? _configureServicesDelegate;
    private string[]? _args;
    private Action<WildCocoLiteAppOptions>? _configureOptions;
    private List<Action<IWildCocoCommandsBuilder>> _configureApp;

    public WildCocoLiteAppHostBuilder(string[]? args)
    {
        _args = args;
        _configureApp = new List<Action<IWildCocoCommandsBuilder>>();
    }

    /// <summary>
    /// Adds services to the container.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public WildCocoLiteAppHostBuilder ConfigureServices(Action<IWildCocoLiteServiceCollection> configureDelegate)
    {
        _configureServicesDelegate ??= _ => { };
        _configureServicesDelegate += configureDelegate;

        return this;
    }

    public WildCocoLiteAppHostBuilder ConfigureArguments(string[]? args)
    {
        _args = args;
        return this;
    }

    public WildCocoLiteAppHostBuilder ConfigureCommandTypes(Type[] commandTypes)
    {
        _configureApp.Add(x =>
        {
            x.AddCommands(commandTypes);
        });

        return this;
    }

    public WildCocoLiteAppHostBuilder ConfigureApplication(Action<IWildCocoCommandsBuilder> configure)
    {
        _configureApp.Add(configure ?? throw new ArgumentNullException(nameof(configure)));

        return this;
    }

    public WildCocoLiteAppHostBuilder ConfigureOptions(Action<WildCocoLiteAppOptions>? configureOptions)
    {
        _configureOptions = configureOptions;
        return this;
    }

    public WildCocoLiteAppHost Build()
    {
        var services = new WildCocoLiteServiceCollection();

        var options = new WildCocoLiteAppOptions();
        _configureOptions?.Invoke(options);
        services.AddSingleton(options);

        services.AddWildCocoCore(_args);
        services.AddWildCocoShellCompletion();

        services.AddSingleton<WildCocoLiteAppHostOptions>(new WildCocoLiteAppHostOptions()
        {
            ConfigureApplication = app =>
            {
                foreach (var configure in _configureApp)
                {
                    configure(app);
                }
            }
        });

        _configureServicesDelegate?.Invoke(services);

        var serviceProvider = new WildCocoLiteServiceProvider(services);
        serviceProvider.GetRequiredService<IWildCocoCommandDispatcherPipelineBuilder>()
            .UseMiddleware<HandleExceptionAndExitMiddleware>()
            .UseMiddleware<HandleParameterBindExceptionMiddleware>()
            .UseMiddleware<RejectUnknownOptionsMiddleware>()
            .UseMiddleware<CommandFilterMiddleware>()
            .UseMiddleware((next, sp) => new InitializeWildCocoLiteConsoleAppMiddleware(next, sp.GetRequiredService<IWildCocoAppContextAccessor>()))
            .UseMiddleware<WildCocoCommandInvokeMiddleware>();

        return new WildCocoLiteAppHost(serviceProvider, options);
    }
}
