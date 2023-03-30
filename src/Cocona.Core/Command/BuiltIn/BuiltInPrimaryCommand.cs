using Cocona.Application;
using Cocona.Help;
using System.Reflection;

namespace Cocona.Command.BuiltIn
{
    public class BuiltInPrimaryCommand
    {
        private readonly ICoconaConsoleProvider _console;
        private readonly ICoconaHelpMessageBuilder _helpBuilder;
        private static readonly MethodInfo MethodShowDefaultMessage = typeof(BuiltInPrimaryCommand).GetMethod(nameof(ShowDefaultMessage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        public BuiltInPrimaryCommand(ICoconaConsoleProvider console, ICoconaHelpMessageBuilder helpBuilder)
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
}
