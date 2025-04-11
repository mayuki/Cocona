using WildCoco.Application;
using WildCoco.Command;
using WildCoco.Filters;
using WildCoco.Help.DocumentModel;
using WildCoco.Internal;

namespace WildCoco.Help;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public abstract class TransformHelpAttribute : Attribute, IWildCocoHelpTransformer
{
    public abstract void TransformHelp(HelpMessage helpMessage, CommandDescriptor command);
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class TransformHelpFactoryAttribute : Attribute, IFilterFactory
{
    public Type Transformer { get; }

    public TransformHelpFactoryAttribute(Type transformer)
    {
        Transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
        if (!typeof(IWildCocoHelpTransformer).IsAssignableFrom(transformer))
        {
            throw new ArgumentException($"Transformer Type '{transformer.FullName}' doesn't implement IWildCocoHelpTransformer");
        }
    }

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return (IFilterMetadata)(serviceProvider.GetRequiredService<IWildCocoInstanceActivator>()).GetServiceOrCreateInstance(serviceProvider, Transformer)!;
    }
}
