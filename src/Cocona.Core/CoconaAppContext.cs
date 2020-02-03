using Cocona.Command;
using Cocona.CommandLine;
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
        /// Gets a collection of features.
        /// </summary>
        public CoconaAppFeatureCollection Features { get; }

        public CoconaAppContext(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            Features = new CoconaAppFeatureCollection();
        }
    }
}
