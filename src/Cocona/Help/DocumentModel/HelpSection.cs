using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cocona.Help.DocumentModel
{
    [DebuggerDisplay("Section: Contents={Children.Count,nq}")]
    public class HelpSection : ICoconaHelpSectioningContent, ICoconaHelpFlowContent
    {
        public List<ICoconaHelpFlowContent> Children { get; }
        
        IEnumerable<ICoconaHelpContent> ICoconaHelpSectioningContent.Children => Children;

        public HelpSection(params ICoconaHelpFlowContent[] children)
        {
            Children = new List<ICoconaHelpFlowContent>(children ?? throw new ArgumentNullException(nameof(children)));
        }
    }
}
