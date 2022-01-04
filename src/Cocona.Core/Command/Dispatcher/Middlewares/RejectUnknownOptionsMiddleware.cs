using Cocona.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cocona.Resources;

namespace Cocona.Command.Dispatcher.Middlewares
{
    public class RejectUnknownOptionsMiddleware : CommandDispatcherMiddleware
    {
        private readonly ICoconaConsoleProvider _console;

        public RejectUnknownOptionsMiddleware(CommandDispatchDelegate next, ICoconaConsoleProvider console) : base(next)
        {
            _console = console;
        }

        public override ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            if (ctx.ParsedCommandLine.UnknownOptions.Count > 0)
            {
                if (!ctx.Command.Flags.HasFlag(CommandFlags.IgnoreUnknownOptions))
                {
                    _console.Error.WriteLine(string.Format(Strings.Command_Error_UnknownOption, ctx.ParsedCommandLine.UnknownOptions[0]));
                    return new ValueTask<int>(129);
                }
            }

            return Next(ctx);
        }
    }
}
