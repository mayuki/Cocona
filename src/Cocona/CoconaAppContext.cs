using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Cocona
{
    public class CoconaAppContext
    {
        public CancellationToken CancellationToken { get; }
        public ILogger Logger { get; }

        public CoconaAppContext(CancellationToken cancellationToken, ILogger logger)
        {
            CancellationToken = cancellationToken;
            Logger = logger;
        }
    }
}
