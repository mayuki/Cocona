using Cocona.CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Cocona.Command.Dispatcher
{
    public interface ICoconaCommandMatcher
    {
        bool TryGetCommand(string commandName, CommandCollection commandCollection, [NotNullWhen(true)] out CommandDescriptor? command);
        CommandDescriptor ResolveOverload(CommandDescriptor command, ParsedCommandLine parsedCommandLine);
    }

    public class CoconaCommandMatcher : ICoconaCommandMatcher
    {
        public bool TryGetCommand(string commandName, CommandCollection commandCollection, [NotNullWhen(true)] out CommandDescriptor? command)
        {
            var matchedCommand = commandCollection.All
                .FirstOrDefault(x =>
                    string.Compare(x.Name, commandName, StringComparison.OrdinalIgnoreCase) == 0 ||
                    x.Aliases.Any(y => string.Compare(y, commandName, StringComparison.OrdinalIgnoreCase) == 0)
                );

            if (matchedCommand != null)
            {
                command = matchedCommand;
                return true;
            }

            command = null;
            return false;
        }

        public CommandDescriptor ResolveOverload(CommandDescriptor command, ParsedCommandLine parsedCommandLine)
        {
            var valueByOption = parsedCommandLine.Options.ToDictionary(k => k.Option, v => v);

            var resolvedCommand = default(CommandDescriptor);
            foreach (var overloadCommand in command.Overloads)
            {
                if (valueByOption.TryGetValue(overloadCommand.Option, out var value))
                {
                    if ((overloadCommand.Value == null) || (value.Value != null && overloadCommand.Comparer.Equals(value.Value, overloadCommand.Value)))
                    {
                        if (resolvedCommand != null) throw new CoconaException($"Command '{command.Name}' and option '{overloadCommand.Option}' has option overloads more than one.");
                        resolvedCommand = overloadCommand.Command;
                        continue;
                    }
                }
            }

            return resolvedCommand ?? command;
        }
    }
}
