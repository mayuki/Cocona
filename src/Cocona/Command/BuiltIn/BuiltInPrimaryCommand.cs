using Cocona.Application;
using Cocona.Help;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Command.BuiltIn
{
    public class BuiltInPrimaryCommand
    {
        private readonly ICoconaConsoleProvider _console;
        private readonly ICoconaCommandHelpProvider _commandHelpProvider;
        private readonly ICoconaHelpRenderer _helpRenderer;
        private readonly ICoconaCommandProvider _commandProvider;

        public BuiltInPrimaryCommand(ICoconaConsoleProvider console, ICoconaCommandHelpProvider commandHelpProvider, ICoconaHelpRenderer helpRenderer, ICoconaCommandProvider commandProvider)
        {
            _console = console;
            _commandHelpProvider = commandHelpProvider;
            _helpRenderer = helpRenderer;
            _commandProvider = commandProvider;
        }

        public static CommandDescriptor GetCommand(string description)
        {
            var t = typeof(BuiltInPrimaryCommand);
            var method = t.GetMethod(nameof(ShowDefaultMessage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            return new CommandDescriptor(
                method,
                method.Name,
                Array.Empty<string>(),
                description,
                Array.Empty<CommandParameterDescriptor>(),
                Array.Empty<CommandOptionDescriptor>(),
                Array.Empty<CommandArgumentDescriptor>(),
                Array.Empty<CommandOverloadDescriptor>(),
                isPrimaryCommand: true
            );
        }

        private void ShowDefaultMessage()
        {
            _console.Output.Write(_helpRenderer.Render(_commandHelpProvider.CreateCommandsIndexHelp(_commandProvider.GetCommandCollection())));
        }
    }
}
