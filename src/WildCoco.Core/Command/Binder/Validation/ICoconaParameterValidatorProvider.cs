namespace WildCoco.Command.Binder.Validation;

public interface IWildCocoParameterValidatorProvider
{
    IEnumerable<IWildCocoParameterValidator> CreateValidators(ICommandParameterDescriptor parameter);
}
