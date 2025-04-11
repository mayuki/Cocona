using WildCoco.Builder;
using WildCoco.Lite.Hosting;
using WildCoco.Builder;
using WildCoco.Lite.Hosting;

namespace WildCoco;

/// <summary>
/// Initializes and starts a WildCoco enabled application.
/// </summary>
public partial class WildCocoLiteApp : IWildCocoCommandsBuilder, IWildCocoAppBuilder
{
    private readonly IWildCocoCommandsBuilder _builder;
    private readonly WildCocoLiteAppHost _host;

    public IServiceProvider Services => _host.Services;

    public WildCocoLiteApp(WildCocoLiteAppHost host)
    {
        _host = host;
        _builder = new WildCocoCommandsBuilder();
    }

    IDictionary<string, object?> IWildCocoCommandsBuilder.Properties => _builder.Properties;
    IWildCocoCommandsBuilder IWildCocoCommandsBuilder.New() => _builder.New();
    IReadOnlyList<ICommandData> IWildCocoCommandsBuilder.Build() => _builder.Build();
    IWildCocoCommandsBuilder IWildCocoCommandsBuilder.Add(ICommandDataSource commandDataSource) => _builder.Add(commandDataSource);


    public Task RunAsync(CancellationToken cancellationToken = default)
        => _host.RunAsync(cancellationToken);
    public void Run()
        => _host.RunAsync(default).GetAwaiter().GetResult();
}
