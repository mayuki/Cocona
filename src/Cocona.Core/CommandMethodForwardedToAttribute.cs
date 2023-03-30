namespace Cocona;

/// <summary>
/// Specifies that a method of the command is forwarded to another method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CommandMethodForwardedToAttribute : Attribute
{
    public Type CommandType { get; }
    public string CommandMethodName { get; }

    public CommandMethodForwardedToAttribute(Type commandType, string commandMethodName)
    {
        CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
        CommandMethodName = commandMethodName ?? throw new ArgumentNullException(nameof(commandMethodName));
    }
}