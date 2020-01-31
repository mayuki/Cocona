using Cocona.Command;
using Cocona.Filters;
using Cocona.Help.DocumentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Help
{
    public interface ICoconaHelpTransformer : IFilterMetadata
    {
        void TransformHelp(HelpMessage helpMessage, CommandDescriptor command);
    }
}
