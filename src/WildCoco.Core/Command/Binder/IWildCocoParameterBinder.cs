using WildCoco.CommandLine;

namespace WildCoco.Command.Binder;

public interface IWildCocoParameterBinder
{
    object?[] Bind(CommandDescriptor commandDescriptor, IReadOnlyList<CommandOption> commandOptionValues, IReadOnlyList<CommandArgument> commandArgumentValues);
}
