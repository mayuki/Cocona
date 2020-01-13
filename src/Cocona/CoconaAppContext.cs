using Cocona.Command;
using Cocona.CommandLine;
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
        public CommandDescriptor ExecutingCommand { get; }

        public CoconaAppContext(CancellationToken cancellationToken, ILogger logger, CommandDescriptor executingCommand)
        {
            CancellationToken = cancellationToken;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ExecutingCommand = executingCommand ?? throw new ArgumentNullException(nameof(executingCommand));
        }
    }
}
