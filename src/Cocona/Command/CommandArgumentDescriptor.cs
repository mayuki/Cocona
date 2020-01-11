using System;
using System.Diagnostics;

namespace Cocona.Command
{
    [DebuggerDisplay("Argument: {Name,nq} (ArgumentType={ArgumentType.FullName,nq}; IsRequired={IsRequired,nq})")]
    public class CommandArgumentDescriptor : CommandParameterDescriptor
    {
        public Type ArgumentType { get; }
        public string Name { get; }
        public int Order { get; }
        public string Description { get; }
        public CoconaDefaultValue DefaultValue { get; }

        public bool IsArray => ArgumentType.IsArray;
        public bool IsRequired => !DefaultValue.HasValue;

        public CommandArgumentDescriptor(Type argumentType, string name, int order, string description, CoconaDefaultValue defaultValue)
        {
            ArgumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Order = order;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            DefaultValue = defaultValue;
        }
    }
}
