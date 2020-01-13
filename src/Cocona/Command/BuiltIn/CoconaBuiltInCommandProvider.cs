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

            // If the collection has multiple-commands without primary command, use built-in primary command.
            if (commandCollection.All.Count() > 1 && commandCollection.Primary == null)
            {
                return new CommandCollection(commandCollection.All.Concat(new[] { BuiltInPrimaryCommand.GetCommand(string.Empty) }).ToArray());
            }

            return commandCollection;
        }
    }
}
