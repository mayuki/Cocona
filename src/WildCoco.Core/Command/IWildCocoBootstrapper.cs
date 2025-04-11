namespace WildCoco.Command;

public interface IWildCocoBootstrapper
{
    void Initialize();
    ValueTask<int> RunAsync(CancellationToken cancellationToken);
}
