using System.Diagnostics;

namespace Cocona.Help.DocumentModel;

[DebuggerDisplay("Description: {Content,nq}")]
public class HelpDescription : HelpParagraph
{
    public HelpDescription(string content) : base(content)
    {
    }
}