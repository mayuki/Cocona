using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CommandAttribute : Attribute
    {
        public string? Name { get; }
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public string Description { get; set; } = string.Empty;

        public CommandAttribute()
        {
        }

        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
