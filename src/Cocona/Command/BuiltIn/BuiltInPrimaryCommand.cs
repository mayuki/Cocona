using Cocona.Help;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Command.BuiltIn
{
    public class BuiltInPrimaryCommand
    {
        private readonly ICoconaCommandHelpProvider _commandHelpProvider;
        private readonly ICoconaHelpRenderer _helpRenderer;
        private readonly ICoconaCommandProvider _commandProvider;

        public BuiltInPrimaryCommand(ICoconaCommandHelpProvider commandHelpProvider, ICoconaHelpRenderer helpRenderer, ICoconaCommandProvider commandProvider)
        {
            _commandHelpProvider = commandHelpProvider;
            _helpRenderer = helpRenderer;
            _commandProvider = commandProvider;
        }

        public static CommandDescriptor GetCommand()
        {
            var t = typeof(BuiltInPrimaryCommand);
            var method = t.GetMethod(nameof(ShowDefaultMessage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            return new CommandDescriptor(
                method,
                method.Name,
                Array.Empty<string>(),
                "Show help message",
                new CommandParameterDescriptor[]{},
                isPrimaryCommand: true
            );
        }

        private void ShowDefaultMessage()
        {
            Console.Write(_helpRenderer.Render(_commandHelpProvider.CreateCommandsIndexHelp(_commandProvider.GetCommandCollection())));
        }
    }
}
