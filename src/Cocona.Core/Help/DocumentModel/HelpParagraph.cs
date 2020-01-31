using System;
using System.Diagnostics;

namespace Cocona.Help.DocumentModel
{
    [DebuggerDisplay("Paragraph: {Content,nq}")]
    public class HelpParagraph : ICoconaHelpFlowContent
    {
        public string Content { get; }

        public HelpParagraph(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}
