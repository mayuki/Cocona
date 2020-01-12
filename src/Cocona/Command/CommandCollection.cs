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

            var primaries = commands.Where(x => x.IsPrimaryCommand).ToArray();
            if (primaries.Length > 0)
            {
                if (primaries.Length > 1)
                {
                    throw new CoconaException($"The commands contains more then one primary command. A primary command must be only one.: {string.Join(", ", primaries.Select(x => x.Name))}");
                }
                Primary = primaries[0];
            }
        }
    }
}
