namespace Cocona;

/// <summary>
/// Specifies the method that should be treated as a command.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class CommandAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the command name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    ///  Gets or sets the command aliases.
    /// </summary>
    public string[] Aliases { get; set; } = Array.Empty<string>();

    /// <summary>
    ///  Gets or sets the command description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    public CommandAttribute()
    {
    }

    public CommandAttribute(string name)
    {
        Name = name;
    }
}