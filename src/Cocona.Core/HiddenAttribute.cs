namespace Cocona;

/// <summary>
/// Specifies the parameter or method that should be hidden from help and usage.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class HiddenAttribute : Attribute
{
}