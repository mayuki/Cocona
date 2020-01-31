using System;
using System.Threading.Tasks;

namespace Cocona.Filters
{
    /// <summary>
    /// An abstract filter that surrounds execution of command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class CommandFilterAttribute : Attribute, ICommandFilter
    {
        public abstract ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next);
    }
}