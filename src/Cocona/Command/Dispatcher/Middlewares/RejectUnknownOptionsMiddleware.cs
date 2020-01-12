using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher.Middlewares
{
    public class RejectUnknownOptionsMiddleware : CommandDispatcherMiddleware
    {
        public RejectUnknownOptionsMiddleware(CommandDispatchDelegate next) : base(next)
        {
        }

        public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            if (ctx.ParsedCommandLine.UnknownOptions.Count > 0)
            {
                Console.Error.WriteLine($"Error: Unknown option '{ctx.ParsedCommandLine.UnknownOptions[0]}'");
                return new ValueTask<int>(129);
            }

            return Next(ctx);
        }
    }
}
