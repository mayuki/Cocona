using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            // Rewrite all command names as lower-case and inject built-in help and version help
            var newCommands = new CommandDescriptor[commands.Count];
            for (var i = 0; i < commands.Count; i++)
            {
                var command = commands[i];
                newCommands[i] = new CommandDescriptor(
                    command.Method,
                    command.Name,
                    command.Aliases,
                    command.Description,
                    command.Parameters,
                    GetParametersWithBuiltInOptions(command.Options, command.IsPrimaryCommand),
                    command.Arguments,
                    command.Overloads,
                    command.Flags
                );
            }

            return new CommandCollection(newCommands);
        }

        private IReadOnlyList<CommandOptionDescriptor> GetParametersWithBuiltInOptions(IReadOnlyList<CommandOptionDescriptor> options, bool isPrimaryCommand)
        {
            var hasHelp = options.Any(x => string.Equals(x.Name, "help", StringComparison.OrdinalIgnoreCase) || x.ShortName.Any(x => x == 'h'));
            var hasVersion = options.Any(x => string.Equals(x.Name, "version", StringComparison.OrdinalIgnoreCase));

            IEnumerable<CommandOptionDescriptor> newOptions = options;

            if (!hasHelp)
            {
                newOptions = newOptions.Concat(new[] { BuiltInCommandOption.Help });
            }
            if (!hasVersion && isPrimaryCommand)
            {
                newOptions = newOptions.Concat(new[] { BuiltInCommandOption.Version });
            }

            return newOptions.ToArray();
        }
    }
}
