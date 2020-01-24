using Cocona.Application;
using Cocona.Help;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cocona.Command.BuiltIn
{
    public class BuiltInPrimaryCommand
    {
        private readonly ICoconaConsoleProvider _console;
        private readonly ICoconaCommandHelpProvider _commandHelpProvider;
        private readonly ICoconaHelpRenderer _helpRenderer;
        private readonly ICoconaCommandProvider _commandProvider;
        private static readonly MethodInfo _methodShowDefaultMessage = typeof(BuiltInPrimaryCommand).GetMethod(nameof(ShowDefaultMessage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        public BuiltInPrimaryCommand(ICoconaConsoleProvider console, ICoconaCommandHelpProvider commandHelpProvider, ICoconaHelpRenderer helpRenderer, ICoconaCommandProvider commandProvider)
        {
            _console = console;
            _commandHelpProvider = commandHelpProvider;
            _helpRenderer = helpRenderer;
            _commandProvider = commandProvider;
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
                CommandFlags.Primary
            );
        }

        private void ShowDefaultMessage()
        {
            _console.Output.Write(_helpRenderer.Render(_commandHelpProvider.CreateCommandsIndexHelp(_commandProvider.GetCommandCollection())));
        }

        public static bool IsBuiltInCommand(CommandDescriptor command)
        {
            return command.Method == _methodShowDefaultMessage;
        }
    }
}
