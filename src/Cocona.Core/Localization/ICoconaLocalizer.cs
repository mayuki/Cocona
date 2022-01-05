using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Command;

namespace Cocona.Localization
{
    public interface ICoconaLocalizer
    {
        string GetCommandDescription(CommandDescriptor command);
        string GetOptionDescription(CommandDescriptor command, ICommandOptionDescriptor option);
        string GetArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument);
    }
}
