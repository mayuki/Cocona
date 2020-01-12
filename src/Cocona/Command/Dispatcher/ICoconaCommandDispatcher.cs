using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher
{
    public interface ICoconaCommandDispatcher
    {
        ValueTask<int> DispatchAsync();
    }
}
