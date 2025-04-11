namespace WildCoco.Command.Binder.Validation;

public interface IWildCocoParameterValidator
{
    IEnumerable<WildCocoParameterValidationResult> Validate(WildCocoParameterValidationContext ctx);
}
