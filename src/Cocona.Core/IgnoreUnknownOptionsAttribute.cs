using System;

namespace Cocona
{
    /// <summary>
    /// Specifies that the command should ignore unknown options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IgnoreUnknownOptionsAttribute : Attribute
    {
    }
}
