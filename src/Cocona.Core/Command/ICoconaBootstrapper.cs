namespace Cocona.Command;

public interface ICoconaBootstrapper
{
    void Initialize();
    ValueTask<int> RunAsync(CancellationToken cancellationToken);
}