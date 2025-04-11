using WildCoco.Command;

namespace WildCoco.Localization.Internal;

public class WildCocoLocalizerWrapper /*: IWildCocoLocalizer*/
{
    private readonly IWildCocoLocalizer _localizer;

    public WildCocoLocalizerWrapper(IServiceProvider serviceProvider)
    {
        _localizer = (IWildCocoLocalizer)(serviceProvider.GetService(typeof(IWildCocoLocalizer)) ?? ThroughLocalizer.Instance);
    }

    public string GetCommandDescription(CommandDescriptor command)
        => _localizer.GetCommandDescription(command);

    public string GetOptionDescription(CommandDescriptor command, ICommandOptionDescriptor option)
        => _localizer.GetOptionDescription(command, option);

    public string GetArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument)
        => _localizer.GetArgumentDescription(command, argument);

    class ThroughLocalizer : IWildCocoLocalizer
    {
        public static IWildCocoLocalizer Instance { get; } = new ThroughLocalizer();

        public string GetCommandDescription(CommandDescriptor command)
            => command.Description;

        public string GetOptionDescription(CommandDescriptor command, ICommandOptionDescriptor option)
            => option.Description;

        public string GetArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument)
            => argument.Description;
    }
}
