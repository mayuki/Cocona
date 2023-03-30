using Cocona.Internal;
using System.Diagnostics;

namespace Cocona.Command
{
    [DebuggerDisplay("Argument: {Name,nq} (ArgumentType={ArgumentType.FullName,nq}; IsRequired={IsRequired,nq})")]
    public class CommandArgumentDescriptor : ICommandParameterDescriptor
    {
        public Type ArgumentType { get; }
        public Type UnwrappedArgumentType { get; } // ArgumentType = Nullable<bool> --> UnwrappedArgumentType = bool
        public string Name { get; }
        public int Order { get; }
        public string Description { get; }
        public CoconaDefaultValue DefaultValue { get; }
        public IReadOnlyList<Attribute> ParameterAttributes { get; }

        public bool IsEnumerableLike => DynamicListHelper.IsArrayOrEnumerableLike(UnwrappedArgumentType);
        public bool IsRequired => !DefaultValue.HasValue;
        

        public CommandArgumentDescriptor(Type argumentType, string name, int order, string description, CoconaDefaultValue defaultValue, IReadOnlyList<Attribute> parameterAttributes)
        {
            ArgumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
            UnwrappedArgumentType = argumentType.IsValueType && argumentType.IsConstructedGenericType && argumentType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? argumentType.GetGenericArguments()[0]
                : argumentType;

            Name = name ?? throw new ArgumentNullException(nameof(name));
            Order = order;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            DefaultValue = defaultValue;
            ParameterAttributes = parameterAttributes;
        }
    }
}
