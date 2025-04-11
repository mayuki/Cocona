using WildCoco.Command;

namespace WildCoco.Localization;

public interface IWildCocoLocalizer
{
    string GetCommandDescription(CommandDescriptor command);
    string GetOptionDescription(CommandDescriptor command, ICommandOptionDescriptor option);
    string GetArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument);
}
