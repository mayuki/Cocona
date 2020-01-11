using System;
using System.Collections.Generic;

namespace Cocona.Command
{
    public class CommandCollection
    {
        public IReadOnlyList<CommandDescriptor> All { get; }

        public CommandCollection(IReadOnlyList<CommandDescriptor> commands)
        {
            All = commands ?? throw new ArgumentNullException(nameof(commands));
        }
    }
}
