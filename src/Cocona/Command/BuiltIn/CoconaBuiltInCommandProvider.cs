using System;
using System.Collections.Generic;
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

            // Rewrite all command names as lower-case and inject built-in help and version help
            commands = commands
                .Select(x => new CommandDescriptor(
                    x.Method,
                    x.Name.ToLower(),
                    x.Aliases,
                    x.Description,
                    x.Parameters,
                    GetParametersWithBuiltInOptions(x.Options, x.IsPrimaryCommand),
                    x.Arguments,
                    x.Overloads,
                    x.IsPrimaryCommand))
                .ToArray();

            return new CommandCollection(commands);
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
