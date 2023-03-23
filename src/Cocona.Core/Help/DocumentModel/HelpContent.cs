using System.Collections.Generic;

namespace Cocona.Help.DocumentModel
{
    public interface ICoconaHelpContent
    {
    }

    public interface ICoconaHelpFlowContent : ICoconaHelpContent
    {
    }

    public interface ICoconaHelpSectioningContent : ICoconaHelpContent
    {
        IEnumerable<ICoconaHelpContent> Children { get; }
    }

}
