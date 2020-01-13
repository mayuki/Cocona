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

        public static CommandDescriptor GetCommand(string description)
        {
            var t = typeof(BuiltInPrimaryCommand);
            var method = t.GetMethod(nameof(ShowDefaultMessage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            return new CommandDescriptor(
                method,
                method.Name,
                Array.Empty<string>(),
                description,
                new CommandParameterDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(bool), "help", new[] { 'h' }, "Show help message", new CoconaDefaultValue(false), null),
                    new CommandOptionDescriptor(typeof(bool), "version", Array.Empty<char>(), "Show version", new CoconaDefaultValue(false), null)
                },
                Array.Empty<CommandOverloadDescriptor>(),
                isPrimaryCommand: true
            );
        }

        private void ShowDefaultMessage(bool help, bool version)
        {
            if (version)
            {
                Console.Write(_helpRenderer.Render(_commandHelpProvider.CreateVersionHelp()));
            }
            else
            {
                Console.Write(_helpRenderer.Render(_commandHelpProvider.CreateCommandsIndexHelp(_commandProvider.GetCommandCollection())));
            }
        }
    }
}
