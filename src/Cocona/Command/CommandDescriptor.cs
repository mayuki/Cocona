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

        public IReadOnlyList<CommandParameterDescriptor> Parameters { get; }
        public IReadOnlyList<CommandOptionDescriptor> Options { get; }
        public IReadOnlyList<CommandArgumentDescriptor> Arguments { get; }

        public CommandDescriptor(MethodInfo methodInfo, string name, IReadOnlyList<string> aliases, string description, IReadOnlyList<CommandParameterDescriptor> parameters)
        {
            Method = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Aliases = aliases ?? throw new ArgumentNullException(nameof(aliases));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Parameters = parameters?.ToArray() ?? throw new ArgumentNullException(nameof(parameters));

            Options = Parameters.OfType<CommandOptionDescriptor>().ToArray();
            Arguments = Parameters.OfType<CommandArgumentDescriptor>().ToArray();
        }
    }
}
