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
                commands = commands.Append(BuiltInPrimaryCommand.GetCommand(string.Empty)).ToArray();
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
                    command.Options,
                    command.Arguments,
                    command.Overloads,
                    BuildOptionLikeCommands(command),
                    command.Flags,
                    (command.SubCommands != null && command.SubCommands != commandCollection) ? GetWrappedCommandCollection(command.SubCommands, depth + 1) : command.SubCommands
                );
            }

            IReadOnlyList<CommandOptionLikeCommandDescriptor> BuildOptionLikeCommands(CommandDescriptor command)
            {
                IEnumerable<CommandOptionLikeCommandDescriptor> optionLikeCommands = command.OptionLikeCommands;

                // NOTE: ToHashSet() requires .NET Standard 2.1
                var allNames = new HashSet<string>(command.Options.Select(x => x.Name).Concat(command.OptionLikeCommands.Select(x => x.Name)));
                var allShortNames = new HashSet<char>(command.Options.SelectMany(x => x.ShortName).Concat(command.OptionLikeCommands.SelectMany(x => x.ShortName)));

                if (!allNames.Contains(BuiltInOptionLikeCommands.Help.Name) && !allShortNames.Overlaps(BuiltInOptionLikeCommands.Help.ShortName))
                {
                    optionLikeCommands = optionLikeCommands.Append(BuiltInOptionLikeCommands.Help);
                }

                if (command.IsPrimaryCommand && depth == 0)
                {
                    // --completion-candidates, --completion, ... original ..., --version
                    optionLikeCommands = optionLikeCommands.Prepend(BuiltInOptionLikeCommands.Completion).Prepend(BuiltInOptionLikeCommands.CompletionCandidates);

                    if (!allNames.Contains(BuiltInOptionLikeCommands.Version.Name) && !allShortNames.Overlaps(BuiltInOptionLikeCommands.Version.ShortName))
                    {
                        optionLikeCommands = optionLikeCommands.Append(BuiltInOptionLikeCommands.Version);
                    }
                }

                return optionLikeCommands.ToArray();
            }

            return new CommandCollection(newCommands);
        }
    }
}
