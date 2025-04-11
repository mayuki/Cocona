using System.Diagnostics;

namespace WildCoco.Help.DocumentModel;

[DebuggerDisplay("Heading: {Content,nq}")]
public class HelpHeading : IWildCocoHelpFlowContent
{
    public string Content { get; }

    public HelpHeading(string content)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }
}
