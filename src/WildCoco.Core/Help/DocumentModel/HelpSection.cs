using System.Diagnostics;

namespace WildCoco.Help.DocumentModel;

[DebuggerDisplay("Section: Id={Id,nq}; Contents={Children.Count,nq}")]
public class HelpSection : IWildCocoHelpSectioningContent, IWildCocoHelpFlowContent
{
    public List<IWildCocoHelpFlowContent> Children { get; }
        
    public string Id { get; set; }

    IEnumerable<IWildCocoHelpContent> IWildCocoHelpSectioningContent.Children => Children;

    public HelpSection(params IWildCocoHelpFlowContent[] children)
    {
        Id = string.Empty;
        Children = new List<IWildCocoHelpFlowContent>(children ?? throw new ArgumentNullException(nameof(children)));
    }

    public HelpSection(string id, params IWildCocoHelpFlowContent[] children)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Children = new List<IWildCocoHelpFlowContent>(children ?? throw new ArgumentNullException(nameof(children)));
    }
}

public static class HelpSectionId
{
    public const string Usage = "usage";
    public const string Description = "description";
    public const string Arguments = "arguments";
    public const string Options = "options";
    public const string Commands = "commands";
    public const string Example = "example";
}
