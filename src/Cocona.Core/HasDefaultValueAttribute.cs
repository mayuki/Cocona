namespace Cocona
{
    /// <summary>
    /// An attribute indicating that the property or field is optional, for use in classes that implement <see cref="ICommandParameterSet"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class HasDefaultValueAttribute : Attribute
    {
    }
}
