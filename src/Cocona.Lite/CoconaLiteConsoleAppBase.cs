namespace Cocona
{
    /// <summary>
    /// An abstract class that implements for console application and provides some context.
    /// </summary>
    public abstract class CoconaLiteConsoleAppBase
    {
        /// <summary>
        /// Gets a current application context.
        /// </summary>
        public CoconaLiteConsoleAppContext Context { get; internal set; } = default!;
    }

    public class CoconaLiteConsoleAppContext
    {
        private readonly CoconaAppContext _context;

        /// <summary>
        /// Gets a cancellation token to waits for shutdown signal.
        /// </summary>
        public CancellationToken CancellationToken => _context.CancellationToken;

        public CoconaLiteConsoleAppContext(CoconaAppContext ctx)
        {
            _context = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }
    }
}
