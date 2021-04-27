using Cocona.Application;
using Cocona.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Cocona.Command.Features;

namespace Cocona.Command.BuiltIn
{
    public class BuiltInPrimaryCommand
    {
        private readonly ICoconaConsoleProvider _console;
        private readonly ICoconaHelpMessageBuilder _helpBuilder;
        private static readonly MethodInfo _methodShowDefaultMessage = typeof(BuiltInPrimaryCommand).GetMethod(nameof(ShowDefaultMessage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        public BuiltInPrimaryCommand(ICoconaConsoleProvider console, ICoconaHelpMessageBuilder helpBuilder)
        {
            _console = console;
            _helpBuilder = helpBuilder;
        }

        public static CommandDescriptor GetCommand(string description)
        {
            return new CommandDescriptor(
                _methodShowDefaultMessage,
                _methodShowDefaultMessage.Name,
                Array.Empty<string>(),
                description,
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
            return command.Method == _methodShowDefaultMessage;
        }
    }
}
