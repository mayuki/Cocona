using System;
using System.Collections.Generic;

namespace Cocona.Command.Features
{
    public interface ICoconaCommandFeature
    {
        object? CommandInstance { get; }
        CommandDescriptor Command { get; }
        CommandCollection CommandCollection { get; }
        IReadOnlyList<CommandDescriptor> CommandStack { get; }
    }

    public class CoconaCommandFeature : ICoconaCommandFeature
    {
        public object? CommandInstance { get; }
        public CommandDescriptor Command { get; }
        public CommandCollection CommandCollection { get; }
        public IReadOnlyList<CommandDescriptor> CommandStack { get; }

        public CoconaCommandFeature(CommandCollection commandCollection, CommandDescriptor commandDescriptor, IReadOnlyList<CommandDescriptor> commandStack, object? commandInstance)
        {
            CommandCollection = commandCollection ?? throw new ArgumentNullException(nameof(commandCollection));
            Command = commandDescriptor ?? throw new ArgumentNullException(nameof(commandDescriptor));
            CommandInstance = commandInstance;
            CommandStack = commandStack ?? throw new ArgumentNullException(nameof(commandStack));
        }
    }
}
