using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Builder.Metadata
{
    public class CommandAliasesMetadata
    {
        public IReadOnlyList<string> Aliases { get; }
        public CommandAliasesMetadata(IReadOnlyList<string> aliases)
        {
            Aliases = aliases;
        }
    }
}
