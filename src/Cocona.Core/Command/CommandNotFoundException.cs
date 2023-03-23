using System;

namespace Cocona.Command
{
    public class CommandNotFoundException : Exception
    {
        public string Command { get; }
        public CommandCollection ImplementedCommands { get; }

        public CommandNotFoundException(string command, CommandCollection implementedCommands, string message)
            : base(message)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            ImplementedCommands = implementedCommands ?? throw new ArgumentNullException(nameof(implementedCommands));
        }
    }
}
