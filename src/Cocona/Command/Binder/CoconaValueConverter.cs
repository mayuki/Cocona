using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Command.Binder
{
    public class CoconaValueConverter : ICoconaValueConverter
    {
        public object? ConvertTo(Type t, string? value)
        {
            return value;
        }
    }
}
