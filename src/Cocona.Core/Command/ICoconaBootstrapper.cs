using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cocona.Command
{
    public interface ICoconaBootstrapper
    {
        void Initialize();
        ValueTask<int> RunAsync(CancellationToken cancellationToken);
    }
}
