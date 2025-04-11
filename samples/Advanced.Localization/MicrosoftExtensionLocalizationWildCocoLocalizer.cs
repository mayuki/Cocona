using Microsoft.Extensions.Localization;
using WildCoco.Command;
using WildCoco.Localization;

namespace CoconaSample.Advanced.Localization;

public class MicrosoftExtensionLocalizationWildCocoLocalizer : IWildCocoLocalizer
{
    private readonly IStringLocalizerFactory _localizerFactory;

    public MicrosoftExtensionLocalizationWildCocoLocalizer(IStringLocalizerFactory localizerFactory)
    {
        _localizerFactory = localizerFactory;
    }

    private Type ResolveType(Type t)
        => (t.IsNested && t.DeclaringType != null) ? t.DeclaringType : t;

    public string GetCommandDescription(CommandDescriptor command)
        => _localizerFactory.Create(ResolveType(command.CommandType)).GetString(command.Description);

    public string GetOptionDescription(CommandDescriptor command, ICommandOptionDescriptor option)
        => _localizerFactory.Create(ResolveType(command.CommandType)).GetString(option.Description);

    public string GetArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument)
        => _localizerFactory.Create(ResolveType(command.CommandType)).GetString(argument.Description);
}
