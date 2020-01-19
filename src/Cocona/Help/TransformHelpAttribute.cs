using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Help
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TransformHelpAttribute : Attribute
    {
        public Type Transformer { get; }

        public TransformHelpAttribute(Type transformer)
        {
            Transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));

            if (!typeof(ICoconaHelpTransformer).IsAssignableFrom(transformer))
            {
                throw new ArgumentException($"Type '{transformer.FullName}' does not implements {typeof(ICoconaHelpTransformer).FullName}");
            }
        }
    }
}
