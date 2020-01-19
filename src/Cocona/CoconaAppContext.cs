using Cocona.Command;
using Cocona.CommandLine;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Cocona
{
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
        /// Gets a logger for a current command.
        /// </summary>
        public ILogger Logger { get; }

        public CoconaAppContext(CancellationToken cancellationToken, ILogger logger)
        {
            CancellationToken = cancellationToken;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
