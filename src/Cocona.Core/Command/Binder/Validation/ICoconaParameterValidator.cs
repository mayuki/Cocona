namespace Cocona.Command.Binder.Validation
{
    public interface ICoconaParameterValidator
    {
        IEnumerable<CoconaParameterValidationResult> Validate(CoconaParameterValidationContext ctx);
    }
}
