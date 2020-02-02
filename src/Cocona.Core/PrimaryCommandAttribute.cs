using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    /// <summary>
    /// Specifies the method that should be treated as a primary command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class PrimaryCommandAttribute : Attribute
    {
    }
}
