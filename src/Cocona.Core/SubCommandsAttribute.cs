using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    public class SubCommandsAttribute : Attribute
    {
        public Type Type { get; }
        public string? CommandName { get; }

        public SubCommandsAttribute(Type commandsType, string? commandName = null)
        {
            Type = commandsType ?? throw new ArgumentNullException(nameof(commandsType));
            CommandName = commandName;
        }
    }

}
