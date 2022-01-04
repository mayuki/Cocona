using System.Collections.Generic;

namespace Cocona.Builder
{
    public interface ICommandBuilder
    {
        IList<object> Metadata { get; }
    }
}
