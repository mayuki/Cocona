using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cocona.Command
{
    [DebuggerDisplay("ParameterSet: {Name,nq} (ParameterSetType={ParameterSetType.FullName,nq}")]
    public class CommandParameterSetDescriptor : ICommandParameterDescriptor
    {
        public Type ParameterSetType { get; }
        public string Name { get; }
        public IReadOnlyList<Attribute> ParameterAttributes { get; }
        public IReadOnlyList<CommandParameterSetMemberDescriptor> Members { get; }

        public CommandParameterSetDescriptor(Type parameterSetType, string name, IReadOnlyList<Attribute> parameterAttributes, IReadOnlyList<CommandParameterSetMemberDescriptor> members)
        {
            ParameterSetType = parameterSetType ?? throw new ArgumentNullException(nameof(parameterSetType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ParameterAttributes = parameterAttributes ?? throw new ArgumentNullException(nameof(parameterAttributes));
            Members = members ?? throw new ArgumentNullException(nameof(members));
        }
    }

    [DebuggerDisplay("ParameterSet (Parameterized): {Name,nq} (ParameterSetType={ParameterSetType.FullName,nq}")]
    public class CommandParameterizedParameterSetDescriptor : ICommandParameterDescriptor
    {
        public Type ParameterSetType { get; }
        public string Name { get; }
        public IReadOnlyList<Attribute> ParameterAttributes { get; }
        public IReadOnlyList<ICommandParameterDescriptor> Parameters { get; }

        public CommandParameterizedParameterSetDescriptor(Type parameterSetType, string name, IReadOnlyList<Attribute> parameterAttributes, IReadOnlyList<ICommandParameterDescriptor> parameters)
        {
            ParameterSetType = parameterSetType ?? throw new ArgumentNullException(nameof(parameterSetType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ParameterAttributes = parameterAttributes ?? throw new ArgumentNullException(nameof(parameterAttributes));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }
    }
    public class CommandParameterSetMemberDescriptor
    {
        public ICommandParameterDescriptor ParameterDescriptor { get; }
        public Action<object, object?> Setter { get; }

        public CommandParameterSetMemberDescriptor(ICommandParameterDescriptor parameterDescriptor, Action<object, object?> setter)
        {
            ParameterDescriptor = parameterDescriptor;
            Setter = setter;
        }
    }
}
