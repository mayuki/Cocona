using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cocona.Command.Binder.Validation
{
    public class DataAnnotationsParameterValidator : ICoconaParameterValidator
    {
        private readonly ValidationAttribute _attribute;

        public DataAnnotationsParameterValidator(ValidationAttribute attribute)
        {
            _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        }

        public IEnumerable<CoconaParameterValidationResult> Validate(CoconaParameterValidationContext ctx)
        {
            if (ctx.Value is null)
            {
                return new[] { new CoconaParameterValidationResult(ctx.Parameter.Name, "The value must not be null.") };
            }
            
            var validationCtx = new ValidationContext(ctx.Value);
            validationCtx.DisplayName = ctx.Parameter.Name;
            var result = _attribute.GetValidationResult(ctx.Value, validationCtx);
            if (result is not null && result != ValidationResult.Success)
            {
                return new[] { new CoconaParameterValidationResult(ctx.Parameter.Name, result.ErrorMessage ?? string.Empty) };
            }

            return Array.Empty<CoconaParameterValidationResult>();
        }
    }
}
