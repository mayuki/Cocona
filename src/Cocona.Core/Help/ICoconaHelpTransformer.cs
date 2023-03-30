using Cocona.Command;
using Cocona.Filters;
using Cocona.Help.DocumentModel;

namespace Cocona.Help
{
    public interface ICoconaHelpTransformer : IFilterMetadata
    {
        void TransformHelp(HelpMessage helpMessage, CommandDescriptor command);
    }
}
