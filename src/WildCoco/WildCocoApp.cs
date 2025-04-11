using WildCoco.Builder;
using WildCoco.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WildCoco;

/// <summary>
/// Initializes and starts a WildCoco enabled application.
/// </summary>
public partial class WildCocoApp : IWildCocoCommandsBuilder, IWildCocoAppBuilder, IDisposable
{
    private readonly IHost _host;
    private readonly IWildCocoCommandsBuilder _commandsBuilder = new WildCocoCommandsBuilder();

    public IServiceProvider Services => _host.Services;
    public IConfiguration Configuration => _host.Services.GetRequiredService<IConfiguration>();
    public IHostEnvironment Environment => _host.Services.GetRequiredService<IHostEnvironment>();
    public IHostApplicationLifetime Lifetime => _host.Services.GetRequiredService<IHostApplicationLifetime>();
    public ILogger Logger => _host.Services.GetRequiredService<ILogger<WildCocoApp>>();

    #region Implements IWildCocoCommandsBuilder
    IDictionary<string, object?> IWildCocoCommandsBuilder.Properties => _commandsBuilder.Properties;
    IWildCocoCommandsBuilder IWildCocoCommandsBuilder.New() => _commandsBuilder.New();
    IReadOnlyList<ICommandData> IWildCocoCommandsBuilder.Build() => _commandsBuilder.Build();
    IWildCocoCommandsBuilder IWildCocoCommandsBuilder.Add(ICommandDataSource commandDataSource) => _commandsBuilder.Add(commandDataSource);
    #endregion

    public WildCocoApp(IHost host)
    {
        _host = host;
    }

    public void Run()
        => _host.Run();

    public Task RunAsync(CancellationToken cancellationToken = default)
        => _host.RunAsync(cancellationToken);

    public void Dispose()
        => _host.Dispose();
}
