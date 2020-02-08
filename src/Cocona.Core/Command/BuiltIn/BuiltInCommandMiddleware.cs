using Cocona.Application;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher;
using Cocona.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Command.Features;

namespace Cocona.Command.BuiltIn
{
    public class BuiltInCommandMiddleware : CommandDispatcherMiddleware
    {
        private readonly ICoconaHelpRenderer _helpRenderer;
        private readonly ICoconaCommandHelpProvider _commandHelpProvider;
        private readonly ICoconaCommandProvider _commandProvider;
        private readonly ICoconaConsoleProvider _console;
        private readonly ICoconaAppContextAccessor _appContext;

        public BuiltInCommandMiddleware(CommandDispatchDelegate next, ICoconaHelpRenderer helpRenderer, ICoconaCommandHelpProvider commandHelpProvider, ICoconaCommandProvider commandProvider, ICoconaConsoleProvider console, ICoconaAppContextAccessor appContext)
            : base(next)
        {
            _helpRenderer = helpRenderer;
            _commandHelpProvider = commandHelpProvider;
            _commandProvider = commandProvider;
            _console = console;
            _appContext = appContext;
        }

        public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            var hasHelpOption = ctx.ParsedCommandLine.Options.Any(x => x.Option == BuiltInCommandOption.Help);
            if (hasHelpOption)
            {
                var feature = _appContext.Current!.Features.Get<ICoconaCommandFeature>()!;
                var commandCollection = feature.CommandCollection ?? _commandProvider.GetCommandCollection();

                var help = (ctx.Command.IsPrimaryCommand)
                    ? _commandHelpProvider.CreateCommandsIndexHelp(commandCollection, feature.CommandStack)
                    : _commandHelpProvider.CreateCommandHelp(ctx.Command, feature.CommandStack);

                _console.Output.Write(_helpRenderer.Render(help));
                return new ValueTask<int>(129);
            }

            var hasVersionOption = ctx.ParsedCommandLine.Options.Any(x => x.Option == BuiltInCommandOption.Version);
            if (hasVersionOption)
            {
                _console.Output.Write(_helpRenderer.Render(_commandHelpProvider.CreateVersionHelp()));
                return new ValueTask<int>(0);
            }

            return Next(ctx);
        }
    }
}
