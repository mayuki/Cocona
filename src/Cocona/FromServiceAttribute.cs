using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class FromServiceAttribute : Attribute
    {
    }
}
