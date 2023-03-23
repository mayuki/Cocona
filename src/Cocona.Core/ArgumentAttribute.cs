using System;

namespace Cocona
{
    /// <summary>
    /// Specifies the parameter that should be treated as command argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ArgumentAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the argument description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the argument name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the argument order. The order is used only for sorting internally.
        /// </summary>
        public int Order { get; set; } = 0;

        public ArgumentAttribute()
        { }

        public ArgumentAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
