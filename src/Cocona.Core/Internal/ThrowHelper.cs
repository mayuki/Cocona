using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Internal
{
    internal static class ThrowHelper
    {
        public static void ArgumentNull(object value, string name)
        {
            if (value == null) throw new ArgumentNullException(nameof(name));
        }
    }
}
