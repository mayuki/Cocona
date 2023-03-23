using System.Collections.Generic;

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
