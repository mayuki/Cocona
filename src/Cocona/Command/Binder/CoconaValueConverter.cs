using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Cocona.Command.Binder
{
    public class CoconaValueConverter : ICoconaValueConverter
    {
        public object? ConvertTo(Type t, string? value)
        {
            return TypeDescriptor.GetConverter(t).ConvertFrom(value);
        }
    }
}
