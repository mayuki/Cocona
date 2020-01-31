using System;
using System.Diagnostics;

namespace Cocona.Help.DocumentModel
{
    [DebuggerDisplay("Heading: {Content,nq}")]
    public class HelpHeading : ICoconaHelpFlowContent
    {
        public string Content { get; }

        public HelpHeading(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}
