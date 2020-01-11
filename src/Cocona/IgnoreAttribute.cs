using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
    }
}
