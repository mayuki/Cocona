using Cocona.CommandLine;
using System.Collections.Generic;

namespace Cocona.Command.Binder
{
    public interface ICoconaParameterBinder
    {
        object?[] Bind(CommandDescriptor commandDescriptor, IReadOnlyList<CommandOption> commandOptionValues, IReadOnlyList<CommandArgument> commandArgumentValues);
    }
}
