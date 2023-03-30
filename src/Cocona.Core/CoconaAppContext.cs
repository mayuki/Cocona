using Cocona.Command;

namespace Cocona;

/// <summary>
/// Stores commonly used values about an application's command executing in Cocona.
/// </summary>
public class CoconaAppContext
{
    /// <summary>
    /// Gets a cancellation token to waits for shutdown signal.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets a collection of features.
    /// </summary>
    public CoconaAppFeatureCollection Features { get; }

    /// <summary>
    /// Gets a executing command.
    /// </summary>
    public CommandDescriptor ExecutingCommand { get; }

    public CoconaAppContext(CommandDescriptor command, CancellationToken cancellationToken)
    {
        ExecutingCommand = command;
        CancellationToken = cancellationToken;
        Features = new CoconaAppFeatureCollection();
    }
}