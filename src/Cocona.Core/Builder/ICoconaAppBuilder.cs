using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cocona.Builder
{
    public interface ICoconaAppBuilder : ICoconaCommandsBuilder
    {
        void Run();
        Task RunAsync(CancellationToken cancellationToken = default);
    }
}
