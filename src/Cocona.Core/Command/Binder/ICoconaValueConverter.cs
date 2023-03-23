using System;

namespace Cocona.Command.Binder
{
    public interface ICoconaValueConverter
    {
        object? ConvertTo(Type t, string? value);
    }
}
