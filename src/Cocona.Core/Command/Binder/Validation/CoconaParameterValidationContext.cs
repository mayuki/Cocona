namespace Cocona.Command.Binder.Validation;

public struct CoconaParameterValidationContext
{
    public IServiceProvider ServiceProvider { get; }
    public ICommandParameterDescriptor Parameter { get; }
    public object? Value { get; }

    public CoconaParameterValidationContext(IServiceProvider serviceProvider, ICommandParameterDescriptor parameter, object? value)
    {
        ServiceProvider = serviceProvider;
        Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        Value = value;
    }
}
