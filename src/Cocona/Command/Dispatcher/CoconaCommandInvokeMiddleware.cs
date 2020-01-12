using Cocona.Command.Binder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher
{
    public class CoconaCommandInvokeMiddleware : CommandDispatcherMiddleware
    {
        private readonly ICoconaParameterBinder _parameterBinder;

        public CoconaCommandInvokeMiddleware(CommandDispatchDelegate next, ICoconaParameterBinder parameterBinder)
            : base(next)
        {
            _parameterBinder = parameterBinder;
        }

        public override async ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            var invokeArgs = _parameterBinder.Bind(ctx.Command!, ctx.ParsedCommandLine!.Options, ctx.ParsedCommandLine!.Arguments);
            var result = ctx.Command!.Method.Invoke(ctx.CommandTarget, invokeArgs);

            if (result is Task<int> taskOfInt)
            {
                var exitCode = await taskOfInt.ConfigureAwait(false);
                return exitCode;
            }
            else if (result is ValueTask<int> valueTaskOfInt)
            {
                var exitCode = await valueTaskOfInt.ConfigureAwait(false);
                return exitCode;
            }
            else if (result is int exitCode)
            {
                return exitCode;
            }

            return 0;
        }
    }
}
