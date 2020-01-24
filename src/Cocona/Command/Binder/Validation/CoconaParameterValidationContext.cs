using System;

namespace Cocona.Command.Binder.Validation
{
    public struct CoconaParameterValidationContext
    {
        public ICommandParameterDescriptor Parameter { get; }
        public object? Value { get; }

        public CoconaParameterValidationContext(ICommandParameterDescriptor parameter, object? value)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Value = value;
        }
    }
}
