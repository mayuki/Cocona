using System;

namespace Cocona.Internal
{
    internal static class ThrowHelper
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull(object? argument, [System.Runtime.CompilerServices.CallerArgumentExpression("argument")] string? paramName = null)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(argument, paramName);
#else
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }
#endif
        }
    }
}
