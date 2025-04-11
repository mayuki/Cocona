namespace WildCoco.Command.Binder.Validation;

public struct WildCocoParameterValidationContext
{
    public ICommandParameterDescriptor Parameter { get; }
    public object? Value { get; }

    public WildCocoParameterValidationContext(ICommandParameterDescriptor parameter, object? value)
    {
        Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        Value = value;
    }
}
