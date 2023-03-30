namespace Cocona.Command;

public class CommandOverloadDescriptor
{
    public CommandOptionDescriptor Option { get; }
    public string? Value { get; }
    public CommandDescriptor Command { get; }
    public IEqualityComparer<string> Comparer { get; }

    public CommandOverloadDescriptor(CommandOptionDescriptor option, string? value, CommandDescriptor command, IEqualityComparer<string>? comparer)
    {
        Option = option ?? throw new ArgumentNullException(nameof(option));
        Value = value;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        Comparer = comparer ?? StringComparer.OrdinalIgnoreCase;
    }
}