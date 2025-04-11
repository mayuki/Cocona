#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    public class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string paramName) { }
    }
}
#endif
