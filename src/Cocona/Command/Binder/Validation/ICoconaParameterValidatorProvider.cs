using System.Collections.Generic;

namespace Cocona.Command.Binder.Validation
{
    public interface ICoconaParameterValidatorProvider
    {
        IEnumerable<ICoconaParameterValidator> CreateValidators(ICommandParameterDescriptor parameter);
    }
}
