using Cocona.Command;
using Cocona.Help.DocumentModel;

namespace Cocona.Help;

public interface ICoconaCommandHelpProvider
{
    HelpMessage CreateCommandHelp(CommandDescriptor command, IReadOnlyList<CommandDescriptor> subCommandStack);
    HelpMessage CreateCommandsIndexHelp(CommandCollection commandCollection, IReadOnlyList<CommandDescriptor> subCommandStack);
    HelpMessage CreateVersionHelp();
}