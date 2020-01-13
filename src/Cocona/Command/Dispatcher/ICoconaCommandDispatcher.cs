using System.Threading;
using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher
{
    public interface ICoconaCommandDispatcher
    {
        ValueTask<int> DispatchAsync(CancellationToken cancellationToken = default);
    }
}
