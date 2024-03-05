using System.ComponentModel;

namespace Cocona.Command.Binder;

public class CoconaValueConverter : ICoconaValueConverter
{
    public object? ConvertTo(Type t, string? value)
    {
        if (value is null)
        {
            return null;
        }
        else if (t == typeof(bool))
        {
            return value != null && string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }
        else if (t == typeof(int))
        {
            return value is null ? 0 : int.Parse(value);
        }
        else if (t == typeof(string))
        {
            return value;
        }
        else if (t == typeof(FileInfo))
        {
            return value is null ? null : new FileInfo(value);
        }
        else if (t == typeof(DirectoryInfo))
        {
            return value is null ? null : new DirectoryInfo(value);
        }

        return TypeDescriptor.GetConverter(t).ConvertFrom(value);
    }
}
