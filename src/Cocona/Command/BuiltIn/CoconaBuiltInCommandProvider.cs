using System;
using System.Linq;

namespace Cocona.Command.BuiltIn
{
    public class CoconaBuiltInCommandProvider : ICoconaCommandProvider
    {
        private readonly ICoconaCommandProvider _underlyingCommandProvider;
        private readonly Lazy<CommandCollection> _commandCollection;

        public CoconaBuiltInCommandProvider(ICoconaCommandProvider underlyingCommandProvider)
        {
            _underlyingCommandProvider = underlyingCommandProvider;
            _commandCollection = new Lazy<CommandCollection>(GetCommandCollectionCore);
        }

        public CommandCollection GetCommandCollection()
            => _commandCollection.Value;

        private CommandCollection GetCommandCollectionCore()
        {
            var commandCollection = _underlyingCommandProvider.GetCommandCollection();
            var commands = commandCollection.All;

            // If the collection has multiple-commands without primary command, use built-in primary command.
            if (commandCollection.All.Count() > 1 && commandCollection.Primary == null)
            {
                commands = commands.Concat(new[] { BuiltInPrimaryCommand.GetCommand(string.Empty) }).ToArray();
            }

            // Rewrite all command names as lower-case.
            return new CommandCollection(commands.Select(x => new CommandDescriptor(x.Method, x.Name.ToLower(), x.Aliases, x.Description, x.Parameters, x.Overloads, x.IsPrimaryCommand)).ToArray());
        }
    }
}
