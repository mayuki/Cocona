namespace WildCoco.Command.Binder.Validation;

public class WildCocoParameterValidationResult
{
    public string Name { get; }
    public string Message { get; }

    public WildCocoParameterValidationResult(string name, string message)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}
