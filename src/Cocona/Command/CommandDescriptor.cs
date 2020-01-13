using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cocona.Command
{
    [DebuggerDisplay("Command: {Name,nq}; CommandType={CommandType.FullName,nq}; Options={Options.Count,nq}; Arguments={Arguments.Count,nq}")]
    public class CommandDescriptor
    {
        public MethodInfo Method { get; }
        public Type CommandType => Method.DeclaringType;
        public string Name { get; }
        public IReadOnlyList<string> Aliases { get; }
        public string Description { get; }
        public Type ReturnType => Method.ReturnType;
        public bool IsPrimaryCommand { get; }

        public IReadOnlyList<CommandParameterDescriptor> Parameters { get; }
        public IReadOnlyList<CommandOptionDescriptor> Options { get; }
        public IReadOnlyList<CommandArgumentDescriptor> Arguments { get; }
        public IReadOnlyList<CommandOverloadDescriptor> Overloads { get; }

        public CommandDescriptor(
            MethodInfo methodInfo,
            string name,
            IReadOnlyList<string> aliases,
            string description,
            IReadOnlyList<CommandParameterDescriptor> parameters,
            IReadOnlyList<CommandOverloadDescriptor> overloads,
            bool isPrimaryCommand
        )
            : this
            (
                methodInfo,
                name,
                aliases,
                description,
                parameters,
                parameters.OfType<CommandOptionDescriptor>().ToArray(),
                parameters.OfType<CommandArgumentDescriptor>().ToArray(),
                overloads,
                isPrimaryCommand
            )
        {
        }

        public CommandDescriptor(
            MethodInfo methodInfo,
            string name,
            IReadOnlyList<string> aliases,
            string description,
            IReadOnlyList<CommandParameterDescriptor> parameters,
            IReadOnlyList<CommandOptionDescriptor> options,
            IReadOnlyList<CommandArgumentDescriptor> arguments,
            IReadOnlyList<CommandOverloadDescriptor> overloads,
            bool isPrimaryCommand
        )
        {
            Method = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Aliases = aliases ?? throw new ArgumentNullException(nameof(aliases));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Parameters = parameters?.ToArray() ?? throw new ArgumentNullException(nameof(parameters));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            Overloads = overloads ?? throw new ArgumentNullException(nameof(overloads));
            IsPrimaryCommand = isPrimaryCommand;
        }
    }
}
