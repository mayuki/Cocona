using WildCoco.Application;
using WildCoco.Help;
using System.Reflection;

namespace WildCoco.Command.BuiltIn;

public class BuiltInPrimaryCommand
{
    private readonly IWildCocoConsoleProvider _console;
    private readonly IWildCocoHelpMessageBuilder _helpBuilder;
    private static readonly MethodInfo MethodShowDefaultMessage = typeof(BuiltInPrimaryCommand).GetMethod(nameof(ShowDefaultMessage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

    public BuiltInPrimaryCommand(IWildCocoConsoleProvider console, IWildCocoHelpMessageBuilder helpBuilder)
    {
        _console = console;
        _helpBuilder = helpBuilder;
    }

    public static CommandDescriptor GetCommand(string description)
    {
        return new CommandDescriptor(
            MethodShowDefaultMessage,
            default,
            MethodShowDefaultMessage.Name,
            Array.Empty<string>(),
            description,
            Array.Empty<object>(),
            Array.Empty<ICommandParameterDescriptor>(),
            Array.Empty<CommandOptionDescriptor>(),
            Array.Empty<CommandArgumentDescriptor>(),
            Array.Empty<CommandOverloadDescriptor>(),
            Array.Empty<CommandOptionLikeCommandDescriptor>(),
            CommandFlags.Primary,
            null
        );
    }

    private void ShowDefaultMessage()
    {
        _console.Output.Write(_helpBuilder.BuildAndRenderForCurrentContext());
    }

    public static bool IsBuiltInCommand(CommandDescriptor command)
    {
        return command.Method == MethodShowDefaultMessage;
    }
}
