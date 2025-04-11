using System.Diagnostics;

namespace WildCoco.Help.DocumentModel;

[DebuggerDisplay("Paragraph: {Content,nq}")]
public class HelpParagraph : IWildCocoHelpFlowContent
{
    public string Content { get; }

    public HelpParagraph(string content)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }
}
