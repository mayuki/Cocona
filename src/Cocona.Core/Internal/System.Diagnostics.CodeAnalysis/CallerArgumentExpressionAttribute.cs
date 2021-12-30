using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    public class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string paramName) { }
    }
}
#endif
