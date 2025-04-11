using System.Diagnostics;

namespace WildCoco.Help.DocumentModel;

[DebuggerDisplay("HelpPreformattedText: {Content,nq}")]
public class HelpPreformattedText : IWildCocoHelpFlowContent
{
    public string Content { get; }

    public HelpPreformattedText(string content)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }
}
