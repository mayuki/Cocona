using WildCoco.Help.DocumentModel;

namespace WildCoco.Help;

public interface IWildCocoHelpRenderer
{
    string Render(HelpMessage message);
}
