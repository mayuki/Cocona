using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Command.Binder
{
    public interface ICoconaValueConverter
    {
        object? ConvertTo(Type t, string? value);
    }
}
