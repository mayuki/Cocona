using System.Diagnostics;

namespace WildCoco.Help.DocumentModel;

[DebuggerDisplay("HelpMessage: Children={Children.Count,nq}")]
public class HelpMessage : IWildCocoHelpContent, IWildCocoHelpSectioningContent
{
    public List<IWildCocoHelpSectioningContent> Children { get; }

    IEnumerable<IWildCocoHelpContent> IWildCocoHelpSectioningContent.Children => Children;

    public HelpMessage(params IWildCocoHelpSectioningContent[] children)
    {
        Children = new List<IWildCocoHelpSectioningContent>(children ?? throw new ArgumentNullException(nameof(children)));
    }
}
