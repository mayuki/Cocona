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
