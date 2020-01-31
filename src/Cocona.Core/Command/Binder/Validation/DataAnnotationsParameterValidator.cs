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
            var validationCtx = new ValidationContext(ctx.Value);
            validationCtx.DisplayName = ctx.Parameter.Name;
            var result = _attribute.GetValidationResult(ctx.Value, validationCtx);
            if (result != ValidationResult.Success)
            {
                return new[] { new CoconaParameterValidationResult(ctx.Parameter.Name, result.ErrorMessage) };
            }

            return Array.Empty<CoconaParameterValidationResult>();
        }
    }
}
