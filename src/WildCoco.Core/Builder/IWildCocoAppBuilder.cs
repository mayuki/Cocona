namespace WildCoco.Builder;

public interface IWildCocoAppBuilder : IWildCocoCommandsBuilder
{
    void Run();
    Task RunAsync(CancellationToken cancellationToken = default);
}
