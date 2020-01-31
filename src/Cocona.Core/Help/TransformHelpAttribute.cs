using System;
using System.Collections.Generic;
using System.Text;
using Cocona.Application;
using Cocona.Command;
using Cocona.Filters;
using Cocona.Help.DocumentModel;

namespace Cocona.Help
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class TransformHelpAttribute : Attribute, ICoconaHelpTransformer
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
            if (!typeof(ICoconaHelpTransformer).IsAssignableFrom(transformer))
            {
                throw new ArgumentException($"Transformer Type '{transformer.FullName}' doesn't implement ICoconaHelpTransformer");
            }
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return (IFilterMetadata)((ICoconaInstanceActivator)serviceProvider.GetService(typeof(ICoconaInstanceActivator))).GetServiceOrCreateInstance(serviceProvider, Transformer)!;
        }
    }
}
