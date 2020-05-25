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
                    (
                        (command.IsPrimaryCommand && depth == 0)
                            ? command.OptionLikeCommands.Concat(new [] { BuiltInOptionLikeCommands.CompletionCandidates, BuiltInOptionLikeCommands.Completion, BuiltInOptionLikeCommands.Help, BuiltInOptionLikeCommands.Version })
                            : command.OptionLikeCommands.Append(BuiltInOptionLikeCommands.Help)
                    ).ToArray(),
                    command.Flags,
                    (command.SubCommands != null && command.SubCommands != commandCollection) ? GetWrappedCommandCollection(command.SubCommands, depth + 1) : command.SubCommands
                );
            }

            return new CommandCollection(newCommands);
        }
    }
}
