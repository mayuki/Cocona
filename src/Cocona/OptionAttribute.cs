using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class OptionAttribute : Attribute
    {
        public string Description { get; set; } = string.Empty;
        public string? Name { get; }
        public IReadOnlyList<char> ShortNames { get; } = Array.Empty<char>();

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
