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
        public IReadOnlyList<CommandParameterSetMemberDescriptor> MemberDescriptors { get; }

        public CommandParameterSetDescriptor(Type parameterSetType, string name, IReadOnlyList<Attribute> parameterAttributes, IReadOnlyList<CommandParameterSetMemberDescriptor> memberDescriptors)
        {
            ParameterSetType = parameterSetType ?? throw new ArgumentNullException(nameof(parameterSetType));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ParameterAttributes = parameterAttributes ?? throw new ArgumentNullException(nameof(parameterAttributes));
            MemberDescriptors = memberDescriptors ?? throw new ArgumentNullException(nameof(memberDescriptors));
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
