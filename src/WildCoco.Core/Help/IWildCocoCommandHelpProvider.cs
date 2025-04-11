using WildCoco.Command;
using WildCoco.Help.DocumentModel;

namespace WildCoco.Help;

public interface IWildCocoCommandHelpProvider
{
    HelpMessage CreateCommandHelp(CommandDescriptor command, IReadOnlyList<CommandDescriptor> subCommandStack);
    HelpMessage CreateCommandsIndexHelp(CommandCollection commandCollection, IReadOnlyList<CommandDescriptor> subCommandStack);
    HelpMessage CreateVersionHelp();
}
