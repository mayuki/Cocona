using System;
using System.Collections.Generic;

namespace Cocona
{
    /// <summary>
    /// Specifies the parameter that should be treated as command option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class OptionAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the option description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets the option name. The name is long-form name. (e.g. "output", "force")
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets or sets the option short-form names. (e.g. 'O', 'f')
        /// </summary>
        public IReadOnlyList<char> ShortNames { get; } = Array.Empty<char>();

        /// <summary>
        /// Gets the option value name.
        /// </summary>
        public string? ValueName { get; set; }

        /// <summary>
        /// Gets or sets whether or not to stop parsing options after this option on the command line.
        /// </summary>
        public bool StopParsingOptions { get; set; }

        public OptionAttribute()
        { }

        public OptionAttribute(string name, char[]? shortNames = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ShortNames = shortNames ?? Array.Empty<char>();

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A name of option must have name.");
        }

        public OptionAttribute(char shortName)
        {
            ShortNames = new[] { shortName };
        }

        public OptionAttribute(char[]? shortNames)
        {
            ShortNames = shortNames ?? Array.Empty<char>();
        }
    }
}
