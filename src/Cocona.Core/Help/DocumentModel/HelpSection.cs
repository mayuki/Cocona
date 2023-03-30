using System.Diagnostics;

namespace Cocona.Help.DocumentModel;

[DebuggerDisplay("Section: Id={Id,nq}; Contents={Children.Count,nq}")]
public class HelpSection : ICoconaHelpSectioningContent, ICoconaHelpFlowContent
{
    public List<ICoconaHelpFlowContent> Children { get; }
        
    public string Id { get; set; }

    IEnumerable<ICoconaHelpContent> ICoconaHelpSectioningContent.Children => Children;

    public HelpSection(params ICoconaHelpFlowContent[] children)
    {
        Id = string.Empty;
        Children = new List<ICoconaHelpFlowContent>(children ?? throw new ArgumentNullException(nameof(children)));
    }

    public HelpSection(string id, params ICoconaHelpFlowContent[] children)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Children = new List<ICoconaHelpFlowContent>(children ?? throw new ArgumentNullException(nameof(children)));
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