using System.ComponentModel.DataAnnotations;

namespace Cocona.Command.Binder.Validation
{
    public class DataAnnotationsParameterValidatorProvider : ICoconaParameterValidatorProvider
    {
        public IEnumerable<ICoconaParameterValidator> CreateValidators(ICommandParameterDescriptor parameter)
        {
            foreach (var attr in parameter.ParameterAttributes)
            {
                if (attr is ValidationAttribute validationAttr)
                {
                    yield return new DataAnnotationsParameterValidator(validationAttr);
                }
            }
        }
    }
}
