using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cocona.Command
{
    [DebuggerDisplay("Command: {Name,nq}; CommandType={CommandType.FullName,nq}; Options={Options.Count,nq}; Arguments={Arguments.Count,nq}; Flags={Flags,nq}")]
    public class CommandDescriptor
    {
        public MethodInfo Method { get; }
        public Type CommandType => Method.DeclaringType;
        public string Name { get; }
        public IReadOnlyList<string> Aliases { get; }
        public string Description { get; }
        public Type ReturnType => Method.ReturnType;
        
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
            string name,
            IReadOnlyList<string> aliases,
            string description,
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
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Aliases = aliases ?? throw new ArgumentNullException(nameof(aliases));
            Description = description ?? throw new ArgumentNullException(nameof(description));
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
        OptionLike = 1 << 3,
    }
}
