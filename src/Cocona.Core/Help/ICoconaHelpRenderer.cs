using Cocona.Help.DocumentModel;

namespace Cocona.Help;

public interface ICoconaHelpRenderer
{
    string Render(HelpMessage message);
}