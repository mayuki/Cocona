using Cocona.Application;
using Cocona.Command;
using Cocona.Command.Features;
using Cocona.Help.DocumentModel;

namespace Cocona.Help;

public class CoconaHelpMessageBuilder : ICoconaHelpMessageBuilder
{
    private readonly ICoconaAppContextAccessor _appContext;
    private readonly ICoconaCommandHelpProvider _commandHelpProvider;
    private readonly ICoconaHelpRenderer _helpRenderer;
    private readonly ICoconaCommandProvider _commandProvider;

    public CoconaHelpMessageBuilder(ICoconaAppContextAccessor appContext, ICoconaCommandHelpProvider commandHelpProvider, ICoconaHelpRenderer helpRenderer, ICoconaCommandProvider commandProvider)
    {
        _appContext = appContext;
        _commandHelpProvider = commandHelpProvider;
        _helpRenderer = helpRenderer;
        _commandProvider = commandProvider;
    }

    public string BuildAndRenderForCurrentContext()
    {
        return _helpRenderer.Render(BuildForCurrentContext());
    }

    public HelpMessage BuildForCurrentContext()
    {
        return BuildFromCurrentContextCore(respectCurrentCommand: false);
    }


    public string BuildAndRenderForCurrentCommand()
    {
        return _helpRenderer.Render(BuildForCurrentCommand());
    }

    public HelpMessage BuildForCurrentCommand()
    {
        return BuildFromCurrentContextCore(respectCurrentCommand: true);
    }

    private HelpMessage BuildFromCurrentContextCore(bool respectCurrentCommand)
    {
        var feature = _appContext.Current!.Features.Get<ICoconaCommandFeature>()!;
        var commandCollection = feature.CommandCollection ?? _commandProvider.GetCommandCollection(); // nested or root

        // If `respectCurrentCommand` is `true`, treats the current command as a target.
        // When called by `--help`, the original command is put on the CommandStack.
        // When directly call the method, the CommandStack may be empty.
        var targetCommand = respectCurrentCommand ? feature.Command : feature.CommandStack.LastOrDefault();

        var help = targetCommand is null
            ? _commandHelpProvider.CreateCommandsIndexHelp(commandCollection, Array.Empty<CommandDescriptor>())
            : targetCommand.IsPrimaryCommand || targetCommand.Flags.HasFlag(CommandFlags.SubCommandsEntryPoint)
                ? _commandHelpProvider.CreateCommandsIndexHelp(commandCollection, feature.CommandStack.Take(feature.CommandStack.Count - 1).ToArray())
                : _commandHelpProvider.CreateCommandHelp(targetCommand, feature.CommandStack.Take(feature.CommandStack.Count - 1).ToArray());

        return help;
    }
}