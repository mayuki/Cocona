using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cocona.Internal;

namespace Cocona.Command
{
    public interface ICommandOptionDescriptor
    {
        string Name { get; }
        IReadOnlyList<char> ShortName { get; }
        string Description { get; }
        CommandOptionFlags Flags { get; }
    }

    [DebuggerDisplay("Option: --{Name,nq} (Type={OptionType.FullName,nq}; IsRequired={IsRequired,nq}); Flags={Flags,nq}")]
    public class CommandOptionDescriptor : ICommandOptionDescriptor, ICommandParameterDescriptor
    {
        public Type OptionType { get; }
        public string Name { get; }
        public IReadOnlyList<char> ShortName { get; }
        public string ValueName { get; }
        public string Description { get; }
        public CoconaDefaultValue DefaultValue { get; }
        public IReadOnlyList<Attribute> ParameterAttributes { get; }

        public CommandOptionFlags Flags { get; }
        public bool IsHidden => (Flags & CommandOptionFlags.Hidden) == CommandOptionFlags.Hidden;
        public bool IsRequired => !DefaultValue.HasValue;
        public bool IsEnumerableLike => DynamicListHelper.IsArrayOrEnumerableLike(OptionType);

        public CommandOptionDescriptor(Type optionType, string name, IReadOnlyList<char> shortName, string description, CoconaDefaultValue defaultValue, string? valueName, CommandOptionFlags flags, IReadOnlyList<Attribute> parameterAttributes)
        {
            OptionType = optionType ?? throw new ArgumentNullException(nameof(optionType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            DefaultValue = defaultValue;
            ValueName = valueName ?? OptionType.Name;
            Flags = flags;
            ParameterAttributes = parameterAttributes;

            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A name of the command option must not be empty.", nameof(name));
            if (defaultValue.HasValue && defaultValue.Value != null && defaultValue.Value.GetType() != optionType)
                throw new ArgumentException($"The type of default value is not compatible with type of option.: OptionType={optionType.FullName}; ValueType={defaultValue.Value.GetType().FullName}");
        }
    }

    [Flags]
    public enum CommandOptionFlags
    {
        None = 0,
        Hidden = 1 << 0,
    }
}
