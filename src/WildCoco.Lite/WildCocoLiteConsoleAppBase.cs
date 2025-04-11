namespace WildCoco;

/// <summary>
/// An abstract class that implements for console application and provides some context.
/// </summary>
public abstract class WildCocoLiteConsoleAppBase
{
    /// <summary>
    /// Gets a current application context.
    /// </summary>
    public WildCocoLiteConsoleAppContext Context { get; internal set; } = default!;
}

public class WildCocoLiteConsoleAppContext
{
    private readonly WildCocoAppContext _context;

    /// <summary>
    /// Gets a cancellation token to waits for shutdown signal.
    /// </summary>
    public CancellationToken CancellationToken => _context.CancellationToken;

    public WildCocoLiteConsoleAppContext(WildCocoAppContext ctx)
    {
        _context = ctx ?? throw new ArgumentNullException(nameof(ctx));
    }
}
