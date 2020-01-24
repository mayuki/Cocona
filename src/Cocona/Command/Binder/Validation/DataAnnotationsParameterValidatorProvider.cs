using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cocona.Command.Binder.Validation
{
    public class DataAnnotationsParameterValidatorProvider : ICoconaParameterValidatorProvider
    {
        public IEnumerable<ICoconaParameterValidator> CreateValidators(ICommandParameterDescriptor parameter)
        {
            return parameter
                .ParameterAttributes
                .OfType<ValidationAttribute>()
                .Select(x => new DataAnnotationsParameterValidator(x));
        }
    }
}
