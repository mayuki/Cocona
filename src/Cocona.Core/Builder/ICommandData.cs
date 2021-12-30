using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Builder
{
    public interface ICommandData
    {
        IReadOnlyList<object> Metadata { get; }
    }
}
