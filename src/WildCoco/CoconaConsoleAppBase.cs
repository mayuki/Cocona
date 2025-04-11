using WildCoco;
using Microsoft.Extensions.Logging;

namespace WildCoco;

/// <summary>
/// An abstract class that implements for console application and provides some context.
/// </summary>
public abstract class WildCocoConsoleAppBase
{
    /// <summary>
    /// Gets a current application context.
    /// </summary>
    public WildCocoConsoleAppContext Context { get; internal set; } = default!;
}

public class WildCocoConsoleAppContext
{
    private readonly WildCocoAppContext _context;

    /// <summary>
    /// Gets a cancellation token to waits for shutdown signal.
    /// </summary>
    public CancellationToken CancellationToken => _context.CancellationToken;

    /// <summary>
    /// Gets a logger for a current command.
    /// </summary>
    public ILogger Logger => _context.Features.Get<ILogger>();

    public WildCocoConsoleAppContext(WildCocoAppContext ctx)
    {
        _context = ctx ?? throw new ArgumentNullException(nameof(ctx));
    }
}
