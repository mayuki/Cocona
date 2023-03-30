namespace Cocona
{
    /// <summary>
    /// Specifies what should be ignored during processing command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
    }
}
