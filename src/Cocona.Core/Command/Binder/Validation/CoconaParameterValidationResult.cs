namespace Cocona.Command.Binder.Validation;

public class CoconaParameterValidationResult
{
    public string Name { get; }
    public string Message { get; }

    public CoconaParameterValidationResult(string name, string message)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}