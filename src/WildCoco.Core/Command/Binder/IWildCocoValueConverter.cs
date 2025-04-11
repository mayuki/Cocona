namespace WildCoco.Command.Binder;

public interface IWildCocoValueConverter
{
    object? ConvertTo(Type t, string? value);
}
