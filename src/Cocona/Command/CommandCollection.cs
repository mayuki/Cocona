using System;
using System.Collections.Generic;
using System.Linq;

namespace Cocona.Command
{
    public class CommandCollection
    {
        public IReadOnlyList<CommandDescriptor> All { get; }
        public CommandDescriptor? Primary { get; }

        public string Description { get; } = string.Empty; // TODO:

        public CommandCollection(IReadOnlyList<CommandDescriptor> commands)
        {
            All = commands ?? throw new ArgumentNullException(nameof(commands));
            Primary = commands.SingleOrDefault(x => x.IsPrimaryCommand);
        }
    }
}
