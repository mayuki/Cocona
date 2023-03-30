using Cocona.Application;
using Cocona.Builder;
using Cocona.Command.Dispatcher;
using Cocona.Command.Dispatcher.Middlewares;
using Cocona.Lite.Builder.Internal;

namespace Cocona.Lite.Hosting;

public class CoconaLiteAppHostBuilder
{
    private Action<ICoconaLiteServiceCollection>? _configureServicesDelegate;
    private string[]? _args;
    private Action<CoconaLiteAppOptions>? _configureOptions;
    private List<Action<ICoconaCommandsBuilder>> _configureApp;

    public CoconaLiteAppHostBuilder(string[]? args)
    {
        _args = args;
        _configureApp = new List<Action<ICoconaCommandsBuilder>>();
    }

    /// <summary>
    /// Adds services to the container.
    /// </summary>
    /// <param name="configureDelegate"></param>
    /// <returns></returns>
    public CoconaLiteAppHostBuilder ConfigureServices(Action<ICoconaLiteServiceCollection> configureDelegate)
    {
        _configureServicesDelegate ??= _ => { };
        _configureServicesDelegate += configureDelegate;

        return this;
    }

    public CoconaLiteAppHostBuilder ConfigureArguments(string[]? args)
    {
        _args = args;
        return this;
    }

    public CoconaLiteAppHostBuilder ConfigureCommandTypes(Type[] commandTypes)
    {
        _configureApp.Add(x =>
        {
            x.AddCommands(commandTypes);
        });

        return this;
    }

    public CoconaLiteAppHostBuilder ConfigureApplication(Action<ICoconaCommandsBuilder> configure)
    {
        _configureApp.Add(configure ?? throw new ArgumentNullException(nameof(configure)));

        return this;
    }

    public CoconaLiteAppHostBuilder ConfigureOptions(Action<CoconaLiteAppOptions>? configureOptions)
    {
        _configureOptions = configureOptions;
        return this;
    }

    public CoconaLiteAppHost Build()
    {
        var services = new CoconaLiteServiceCollection();

        var options = new CoconaLiteAppOptions();
        _configureOptions?.Invoke(options);
        services.AddSingleton(options);

        services.AddCoconaCore(_args);
        services.AddCoconaShellCompletion();

        services.AddSingleton<CoconaLiteAppHostOptions>(new CoconaLiteAppHostOptions()
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

        var serviceProvider = new CoconaLiteServiceProvider(services);
        serviceProvider.GetRequiredService<ICoconaCommandDispatcherPipelineBuilder>()
            .UseMiddleware<HandleExceptionAndExitMiddleware>()
            .UseMiddleware<HandleParameterBindExceptionMiddleware>()
            .UseMiddleware<RejectUnknownOptionsMiddleware>()
            .UseMiddleware<CommandFilterMiddleware>()
            .UseMiddleware((next, sp) => new InitializeCoconaLiteConsoleAppMiddleware(next, sp.GetRequiredService<ICoconaAppContextAccessor>()))
            .UseMiddleware<CoconaCommandInvokeMiddleware>();

        return new CoconaLiteAppHost(serviceProvider, options);
    }
}
