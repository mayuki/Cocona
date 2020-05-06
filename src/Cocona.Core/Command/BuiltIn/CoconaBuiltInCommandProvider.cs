using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocona.Command.BuiltIn
{
    public class CoconaBuiltInCommandProvider : ICoconaCommandProvider
    {
        private readonly ICoconaCommandProvider _underlyingCommandProvider;
        private CommandCollection? _cachedCommandCollection;

        public CoconaBuiltInCommandProvider(ICoconaCommandProvider underlyingCommandProvider)
        {
            _underlyingCommandProvider = underlyingCommandProvider;
        }

        public CommandCollection GetCommandCollection()
        {
            return _cachedCommandCollection ??= GetWrappedCommandCollection(_underlyingCommandProvider.GetCommandCollection());
        }

        private CommandCollection GetWrappedCommandCollection(CommandCollection commandCollection, int depth = 0)
        {
            var commands = commandCollection.All;

            // If the collection has multiple-commands without primary command, use built-in primary command.
            if (commandCollection.All.Count > 1 && commandCollection.Primary == null)
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
                    GetParametersWithBuiltInOptions(command.Options, command.IsPrimaryCommand, depth != 0),
                    command.Arguments,
                    command.Overloads,
                    command.Flags,
                    (command.SubCommands != null && command.SubCommands != commandCollection) ? GetWrappedCommandCollection(command.SubCommands, depth + 1) : command.SubCommands
                );
            }

            return new CommandCollection(newCommands);
        }

        private IReadOnlyList<CommandOptionDescriptor> GetParametersWithBuiltInOptions(IReadOnlyList<CommandOptionDescriptor> options, bool isPrimaryCommand, bool isNestedSubCommand)
        {
            var hasHelp = options.Any(x => string.Equals(x.Name, "help", StringComparison.OrdinalIgnoreCase) || x.ShortName.Any(x => x == 'h'));
            var hasVersion = options.Any(x => string.Equals(x.Name, "version", StringComparison.OrdinalIgnoreCase));
            var hasCompletion = options.Any(x => string.Equals(x.Name, "completion", StringComparison.OrdinalIgnoreCase));

            IEnumerable<CommandOptionDescriptor> newOptions = options;

            if (!hasHelp)
            {
                newOptions = newOptions.Concat(new[] { BuiltInCommandOption.Help });
            }
            if (!hasVersion && isPrimaryCommand && !isNestedSubCommand)
            {
                newOptions = newOptions.Concat(new[] { BuiltInCommandOption.Version });
            }
            if (!hasCompletion && isPrimaryCommand && !isNestedSubCommand)
            {
                newOptions = newOptions.Concat(new[] { BuiltInCommandOption.Completion });
            }

            return newOptions.ToArray();
        }
    }
}
