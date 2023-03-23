using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Cocona
{
    /// <summary>
    /// An abstract class that implements for console application and provides some context.
    /// </summary>
    public abstract class CoconaConsoleAppBase
    {
        /// <summary>
        /// Gets a current application context.
        /// </summary>
        public CoconaConsoleAppContext Context { get; internal set; } = default!;
    }

    public class CoconaConsoleAppContext
    {
        private readonly CoconaAppContext _context;

        /// <summary>
        /// Gets a cancellation token to waits for shutdown signal.
        /// </summary>
        public CancellationToken CancellationToken => _context.CancellationToken;

        /// <summary>
        /// Gets a logger for a current command.
        /// </summary>
        public ILogger Logger => _context.Features.Get<ILogger>();

        public CoconaConsoleAppContext(CoconaAppContext ctx)
        {
            _context = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }
    }
}
