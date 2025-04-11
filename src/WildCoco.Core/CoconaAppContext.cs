using WildCoco.Command;

namespace WildCoco;

/// <summary>
/// Stores commonly used values about an application's command executing in WildCoco.
/// </summary>
public class WildCocoAppContext
{
    /// <summary>
    /// Gets a cancellation token to waits for shutdown signal.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets a collection of features.
    /// </summary>
    public WildCocoAppFeatureCollection Features { get; }

    /// <summary>
    /// Gets a executing command.
    /// </summary>
    public CommandDescriptor ExecutingCommand { get; }

    public WildCocoAppContext(CommandDescriptor command, CancellationToken cancellationToken)
    {
        ExecutingCommand = command;
        CancellationToken = cancellationToken;
        Features = new WildCocoAppFeatureCollection();
    }
}
