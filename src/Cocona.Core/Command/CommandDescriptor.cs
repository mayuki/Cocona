using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Cocona.Command
{
    [DebuggerDisplay("Command: {Name,nq}; CommandType={CommandType.FullName,nq}; Options={Options.Count,nq}; Arguments={Arguments.Count,nq}; Flags={Flags,nq}")]
    public class CommandDescriptor
    {
        public MethodInfo Method { get; }
        public object? Target { get; }
        public Type CommandType => Method.DeclaringType ?? throw new InvalidOperationException("The command method must be a member of the class.");
        public string Name { get; }
        public IReadOnlyList<string> Aliases { get; }
        public string Description { get; }
        public Type ReturnType => Method.ReturnType;
        public IReadOnlyList<object> Metadata { get; }
        
        public CommandFlags Flags { get; }
        public bool IsPrimaryCommand => (Flags & CommandFlags.Primary) == CommandFlags.Primary;
        public bool IsHidden => (Flags & CommandFlags.Hidden) == CommandFlags.Hidden;

        public IReadOnlyList<ICommandParameterDescriptor> Parameters { get; }
        public IReadOnlyList<CommandOptionDescriptor> Options { get; }
        public IReadOnlyList<CommandArgumentDescriptor> Arguments { get; }
        public IReadOnlyList<CommandOverloadDescriptor> Overloads { get; }
        public IReadOnlyList<CommandOptionLikeCommandDescriptor> OptionLikeCommands { get; }

        public CommandCollection? SubCommands { get; }

        public CommandDescriptor(
            MethodInfo methodInfo,
            object? target,
            string name,
            IReadOnlyList<string> aliases,
            string description,
            IReadOnlyList<object> metadata,
            IReadOnlyList<ICommandParameterDescriptor> parameters,
            IReadOnlyList<CommandOptionDescriptor> options,
            IReadOnlyList<CommandArgumentDescriptor> arguments,
            IReadOnlyList<CommandOverloadDescriptor> overloads,
            IReadOnlyList<CommandOptionLikeCommandDescriptor> optionLikeCommands,
            CommandFlags flags,
            CommandCollection? subCommands
        )
        {
            Method = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            Target = target;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Aliases = aliases ?? throw new ArgumentNullException(nameof(aliases));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Parameters = parameters?.ToArray() ?? throw new ArgumentNullException(nameof(parameters));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            Overloads = overloads ?? throw new ArgumentNullException(nameof(overloads));
            OptionLikeCommands = optionLikeCommands ?? throw new ArgumentNullException(nameof(optionLikeCommands));
            Flags = flags;
            SubCommands = subCommands;
        }
    }

    [Flags]
    public enum CommandFlags
    {
        None = 0,
        Primary = 1 << 0,
        Hidden = 1 << 1,
        SubCommandsEntryPoint = 1 << 2,
        IgnoreUnknownOptions = 1 << 3,

        /// <summary>
        /// The command has no user-defined command name.
        /// </summary>
        Unnamed = 1 << 4,
    }
}
