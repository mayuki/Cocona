using System.Diagnostics;

namespace Cocona.Help.DocumentModel;

[DebuggerDisplay("HelpMessage: Children={Children.Count,nq}")]
public class HelpMessage : ICoconaHelpContent, ICoconaHelpSectioningContent
{
    public List<ICoconaHelpSectioningContent> Children { get; }

    IEnumerable<ICoconaHelpContent> ICoconaHelpSectioningContent.Children => Children;

    public HelpMessage(params ICoconaHelpSectioningContent[] children)
    {
        Children = new List<ICoconaHelpSectioningContent>(children ?? throw new ArgumentNullException(nameof(children)));
    }
}