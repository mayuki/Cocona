using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cocona.Command
{
    [DebuggerDisplay("Command: {Name,nq}; CommandType={CommandType.FullName,nq}; Options={Options.Count,nq}; Arguments={Arguments.Count,nq}")]
    public class CommandDescriptor
    {
        public Type CommandType { get; }
        public string Name { get; }
        public IReadOnlyList<string> Aliases { get; }
        public string Description { get; }
        public Type ReturnType { get; }

        public IReadOnlyList<CommandParameterDescriptor> Parameters { get; }
        public IReadOnlyList<CommandOptionDescriptor> Options { get; }
        public IReadOnlyList<CommandArgumentDescriptor> Arguments { get; }

        public CommandDescriptor(Type commandType, string name, IReadOnlyList<string> aliases, string description, IReadOnlyList<CommandParameterDescriptor> parameters, Type returnType)
        {
            CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Aliases = aliases ?? throw new ArgumentNullException(nameof(aliases));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Parameters = parameters?.ToArray() ?? throw new ArgumentNullException(nameof(parameters));
            ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));

            Options = Parameters.OfType<CommandOptionDescriptor>().ToArray();
            Arguments = Parameters.OfType<CommandArgumentDescriptor>().ToArray();
        }
    }
}
