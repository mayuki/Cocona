using System.Diagnostics;

namespace Cocona.Help.DocumentModel;

[DebuggerDisplay("HelpPreformattedText: {Content,nq}")]
public class HelpPreformattedText : ICoconaHelpFlowContent
{
    public string Content { get; }

    public HelpPreformattedText(string content)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }
}