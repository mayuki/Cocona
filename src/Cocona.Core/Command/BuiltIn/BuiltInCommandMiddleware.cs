using Cocona.Application;
using Cocona.Command.BuiltIn;
using Cocona.Command.Dispatcher;
using Cocona.Help;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Command.Features;
using Cocona.CommandLine;
using Cocona.ShellCompletion;
using Cocona.ShellCompletion.Candidate;

namespace Cocona.Command.BuiltIn
{
    public class BuiltInCommandMiddleware : CommandDispatcherMiddleware
    {
        private readonly ICoconaHelpRenderer _helpRenderer;
        private readonly ICoconaCommandHelpProvider _commandHelpProvider;
        private readonly ICoconaCommandProvider _commandProvider;
        private readonly ICoconaConsoleProvider _console;
        private readonly ICoconaAppContextAccessor _appContext;
        private readonly ICoconaShellCompletionCodeProvider _shellCompletionCodeGenerator;
        private readonly ICoconaCompletionCandidates _completionCandidates;

        public BuiltInCommandMiddleware(
            CommandDispatchDelegate next,
            ICoconaHelpRenderer helpRenderer,
            ICoconaCommandHelpProvider commandHelpProvider,
            ICoconaCommandProvider commandProvider,
            ICoconaConsoleProvider console,
            ICoconaAppContextAccessor appContext,
            ICoconaShellCompletionCodeProvider shellCompletionCodeGenerator,
            ICoconaCompletionCandidates completionCandidates
        )
            : base(next)
        {
            _helpRenderer = helpRenderer;
            _commandHelpProvider = commandHelpProvider;
            _commandProvider = commandProvider;
            _console = console;
            _appContext = appContext;
            _shellCompletionCodeGenerator = shellCompletionCodeGenerator;
            _completionCandidates = completionCandidates;
        }

        public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            // --completion <shell>
            var hasCompletionOption = ctx.ParsedCommandLine.Options.Any(x => x.Option == BuiltInCommandOption.Completion);
            if (hasCompletionOption)
            {
                var opt = ctx.ParsedCommandLine.Options.First(x => x.Option == BuiltInCommandOption.Completion);
                if (opt.Value is null)
                {
                    _console.Error.Write("Error: Shell name not specified.");
                    return new ValueTask<int>(1);
                }

                if (!_shellCompletionCodeGenerator.CanHandle(opt.Value))
                {
                    _console.Error.Write($"Error: Shell completion for '{opt.Value}' is not supported.");
                    return new ValueTask<int>(1);
                }

                _shellCompletionCodeGenerator.Generate(opt.Value, _console.Output, _commandProvider.GetCommandCollection());
                return new ValueTask<int>(0);
            }

            // --completion-candidates <shell>:<paramName>
            var hasCompletionCandidatesOption = ctx.ParsedCommandLine.Options.Any(x => x.Option == BuiltInCommandOption.CompletionCandidates);
            if (hasCompletionCandidatesOption)
            {
                var opt = ctx.ParsedCommandLine.Options.First(x => x.Option == BuiltInCommandOption.CompletionCandidates);
                var parts = opt.Value!.Split(new[] { ':' }, 2);
                var (shellTarget, paramName) = (parts[0], parts[1]);

                var candidates = _completionCandidates.GetOnTheFlyCandidates(paramName, opt.Position + 1, 0, null);
                _shellCompletionCodeGenerator.GenerateOnTheFlyCandidates(shellTarget, _console.Output, candidates);
                return new ValueTask<int>(0);
            }

            // --help
            var hasHelpOption = ctx.ParsedCommandLine.Options.Any(x => x.Option == BuiltInCommandOption.Help);
            if (hasHelpOption)
            {
                var feature = _appContext.Current!.Features.Get<ICoconaCommandFeature>()!;
                var commandCollection = feature.CommandCollection ?? _commandProvider.GetCommandCollection(); // nested or root

                var help = (ctx.Command.IsPrimaryCommand)
                    ? _commandHelpProvider.CreateCommandsIndexHelp(commandCollection, feature.CommandStack)
                    : _commandHelpProvider.CreateCommandHelp(ctx.Command, feature.CommandStack);

                _console.Output.Write(_helpRenderer.Render(help));
                return new ValueTask<int>(129);
            }

            // --version
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
