using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ArgumentAttribute : Attribute
    {
        public string Description { get; set; } = string.Empty;
        public string? Name { get; set; }

        public int Order { get; set; } = 0;

        public ArgumentAttribute()
        { }

        public ArgumentAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
