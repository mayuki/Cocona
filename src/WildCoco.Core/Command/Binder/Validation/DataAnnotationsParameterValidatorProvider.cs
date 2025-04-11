using System.ComponentModel.DataAnnotations;

namespace WildCoco.Command.Binder.Validation;

public class DataAnnotationsParameterValidatorProvider : IWildCocoParameterValidatorProvider
{
    public IEnumerable<IWildCocoParameterValidator> CreateValidators(ICommandParameterDescriptor parameter)
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
