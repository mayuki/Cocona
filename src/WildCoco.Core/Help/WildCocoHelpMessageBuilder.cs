using WildCoco.Application;
using WildCoco.Command;
using WildCoco.Command.Features;
using WildCoco.Help.DocumentModel;

namespace WildCoco.Help;

public class WildCocoHelpMessageBuilder : IWildCocoHelpMessageBuilder
{
    private readonly IWildCocoAppContextAccessor _appContext;
    private readonly IWildCocoCommandHelpProvider _commandHelpProvider;
    private readonly IWildCocoHelpRenderer _helpRenderer;
    private readonly IWildCocoCommandProvider _commandProvider;

    public WildCocoHelpMessageBuilder(IWildCocoAppContextAccessor appContext, IWildCocoCommandHelpProvider commandHelpProvider, IWildCocoHelpRenderer helpRenderer, IWildCocoCommandProvider commandProvider)
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
        var feature = _appContext.Current!.Features.Get<IWildCocoCommandFeature>()!;
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
