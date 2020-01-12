using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    public class CoconaException : Exception
    {
        public CoconaException(string messge)
            : base(messge)
        {
        }

        public CoconaException(string messge, Exception innerException)
            : base(messge, innerException)
        {
        }
    }
}
