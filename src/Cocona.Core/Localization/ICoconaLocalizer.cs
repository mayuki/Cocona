using Cocona.Command;

namespace Cocona.Localization;

public interface ICoconaLocalizer
{
    string GetCommandDescription(CommandDescriptor command);
    string GetOptionDescription(CommandDescriptor command, ICommandOptionDescriptor option);
    string GetArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument);
}