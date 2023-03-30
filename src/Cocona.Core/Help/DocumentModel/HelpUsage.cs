using System.Diagnostics;

namespace Cocona.Help.DocumentModel;

[DebuggerDisplay("Usage: {Content,nq}")]
public class HelpUsage : HelpHeading
{
    public HelpUsage(string content) : base(content)
    {
    }
}