using System.Text;
using System.Threading.Tasks;

namespace Cocona.Filters
{
    public delegate ValueTask<int> CommandExecutionDelegate(CoconaCommandExecutingContext ctx);

    /// <summary>
    /// An interface for filter that surrounds execution of command.
    /// </summary>
    public interface ICommandFilter : IFilterMetadata
    {
        ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next);
    }
}
