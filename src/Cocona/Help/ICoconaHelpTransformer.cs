using Cocona.Command;
using Cocona.Help.DocumentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Help
{
    public interface ICoconaHelpTransformer
    {
        void TransformHelp(HelpMessage helpMessage, CommandDescriptor command);
    }
}
