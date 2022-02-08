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

            for (var i = 0; i < commands.Count; i++)
            {
                var command = commands[i];
                if (command.IsPrimaryCommand)
                {
                    if (Primary != null)
                    {
                        throw new CoconaException($"The commands contain more than one primary command. A primary command must be unique.: {string.Join(", ", commands.Where(x => x.IsPrimaryCommand).Select(x => x.Name))}");
                    }
                    Primary = command;
                }
            }
        }
    }
}
