using Cocona.Command;

namespace Cocona.Localization.Internal;

public class CoconaLocalizerWrapper /*: ICoconaLocalizer*/
{
    private readonly ICoconaLocalizer _localizer;

    public CoconaLocalizerWrapper(IServiceProvider serviceProvider)
    {
        _localizer = (ICoconaLocalizer)(serviceProvider.GetService(typeof(ICoconaLocalizer)) ?? ThroughLocalizer.Instance);
    }

    public string GetCommandDescription(CommandDescriptor command)
        => _localizer.GetCommandDescription(command);

    public string GetOptionDescription(CommandDescriptor command, ICommandOptionDescriptor option)
        => _localizer.GetOptionDescription(command, option);

    public string GetArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument)
        => _localizer.GetArgumentDescription(command, argument);

    class ThroughLocalizer : ICoconaLocalizer
    {
        public static ICoconaLocalizer Instance { get; } = new ThroughLocalizer();

        public string GetCommandDescription(CommandDescriptor command)
            => command.Description;

        public string GetOptionDescription(CommandDescriptor command, ICommandOptionDescriptor option)
            => option.Description;

        public string GetArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument)
            => argument.Description;
    }
}