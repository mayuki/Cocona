using System.Collections.Generic;

namespace Cocona.Builder
{
    public interface ICommandData
    {
        IReadOnlyList<object> Metadata { get; }
    }
}
