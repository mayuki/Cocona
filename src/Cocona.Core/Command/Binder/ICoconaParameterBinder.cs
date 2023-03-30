using Cocona.CommandLine;

namespace Cocona.Command.Binder;

public interface ICoconaParameterBinder
{
    object?[] Bind(CommandDescriptor commandDescriptor, IReadOnlyList<CommandOption> commandOptionValues, IReadOnlyList<CommandArgument> commandArgumentValues);
}