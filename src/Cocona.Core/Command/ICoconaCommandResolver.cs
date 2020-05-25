using System.Collections.Generic;

namespace Cocona.Command
{
    public interface ICoconaCommandResolver
    {
        CommandResolverResult ParseAndResolve(IReadOnlyList<string> args);
    }
}
