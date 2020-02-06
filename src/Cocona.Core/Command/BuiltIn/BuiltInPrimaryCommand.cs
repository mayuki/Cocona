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
        private readonly ICoconaAppContextAccessor _appContext;
        private readonly ICoconaConsoleProvider _console;
        private readonly ICoconaCommandHelpProvider _commandHelpProvider;
        private readonly ICoconaHelpRenderer _helpRenderer;
        private readonly ICoconaCommandProvider _commandProvider;
        private static readonly MethodInfo _methodShowDefaultMessage = typeof(BuiltInPrimaryCommand).GetMethod(nameof(ShowDefaultMessage), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        public BuiltInPrimaryCommand(ICoconaAppContextAccessor appContext, ICoconaConsoleProvider console, ICoconaCommandHelpProvider commandHelpProvider, ICoconaHelpRenderer helpRenderer, ICoconaCommandProvider commandProvider)
        {
            _appContext = appContext;
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
                CommandFlags.Primary,
                null
            );
        }

        private void ShowDefaultMessage()
        {
            var commandStack = _appContext.Current?.Features.Get<ICoconaNestedCommandFeature>()?.CommandStack;
            var commandCollection = commandStack?.LastOrDefault()?.SubCommands ?? _commandProvider.GetCommandCollection();
            _console.Output.Write(_helpRenderer.Render(_commandHelpProvider.CreateCommandsIndexHelp(commandCollection, commandStack)));
        }

        public static bool IsBuiltInCommand(CommandDescriptor command)
        {
            return command.Method == _methodShowDefaultMessage;
        }
    }
}
