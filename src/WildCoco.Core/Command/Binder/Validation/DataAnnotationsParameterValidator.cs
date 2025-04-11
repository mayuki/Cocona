using System.ComponentModel.DataAnnotations;

namespace WildCoco.Command.Binder.Validation;

public class DataAnnotationsParameterValidator : IWildCocoParameterValidator
{
    private readonly ValidationAttribute _attribute;

    public DataAnnotationsParameterValidator(ValidationAttribute attribute)
    {
        _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
    }

    public IEnumerable<WildCocoParameterValidationResult> Validate(WildCocoParameterValidationContext ctx)
    {
        if (ctx.Value is null)
        {
            return new[] { new WildCocoParameterValidationResult(ctx.Parameter.Name, "The value must not be null.") };
        }
            
        var validationCtx = new ValidationContext(ctx.Value);
        validationCtx.DisplayName = ctx.Parameter.Name;
        var result = _attribute.GetValidationResult(ctx.Value, validationCtx);
        if (result is not null && result != ValidationResult.Success)
        {
            return new[] { new WildCocoParameterValidationResult(ctx.Parameter.Name, result.ErrorMessage ?? string.Empty) };
        }

        return Array.Empty<WildCocoParameterValidationResult>();
    }
}
