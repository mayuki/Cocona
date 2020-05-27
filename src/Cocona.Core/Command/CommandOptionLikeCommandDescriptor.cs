using System;
using System.Collections.Generic;

namespace Cocona.Command
{
    public class CommandOptionLikeCommandDescriptor : ICommandOptionDescriptor
    {
        public string Name { get; }
        public IReadOnlyList<char> ShortName { get; }
        public string Description => Command.Description;
        public CommandOptionFlags Flags { get; }
        public bool IsHidden => (Flags & CommandOptionFlags.Hidden) == CommandOptionFlags.Hidden;

        public CommandDescriptor Command { get; }

        public CommandOptionLikeCommandDescriptor(string name, IReadOnlyList<char> shortName, CommandDescriptor command, CommandOptionFlags commandOptionFlags)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Flags = CommandOptionFlags.OptionLikeCommand | commandOptionFlags;
        }
    }
}
