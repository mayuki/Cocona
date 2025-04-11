using WildCoco.Command;
using WildCoco.Filters;
using WildCoco.Help.DocumentModel;

namespace WildCoco.Help;

public interface IWildCocoHelpTransformer : IFilterMetadata
{
    void TransformHelp(HelpMessage helpMessage, CommandDescriptor command);
}
