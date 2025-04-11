using System.Diagnostics;

namespace WildCoco.Help.DocumentModel;

[DebuggerDisplay("Usage: {Content,nq}")]
public class HelpUsage : HelpHeading
{
    public HelpUsage(string content) : base(content)
    {
    }
}
