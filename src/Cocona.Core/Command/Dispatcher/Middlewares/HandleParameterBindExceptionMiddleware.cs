using Cocona.Application;
using Cocona.Command.Binder;
using Cocona.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Resources;

namespace Cocona.Command.Dispatcher.Middlewares
{
    public class HandleParameterBindExceptionMiddleware : CommandDispatcherMiddleware
    {
        private readonly ICoconaConsoleProvider _console;

        public HandleParameterBindExceptionMiddleware(CommandDispatchDelegate next, ICoconaConsoleProvider console) : base(next)
        {
            _console = console;
        }

        public override async ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            try
            {
                return await Next(ctx);
            }
            catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.InsufficientArgument)
            {
                _console.Error.WriteLine(string.Format(Strings.Command_Error_Insufficient_Argument, paramEx.Argument!.Name));
            }
            catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.InsufficientOption)
            {
                _console.Error.WriteLine(string.Format(Strings.Command_Error_Insufficient_Option, paramEx.Option!.Name));
            }
            catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.InsufficientOptionValue)
            {
                _console.Error.WriteLine(string.Format(Strings.Command_Error_Insufficient_OptionValue, paramEx.Option!.Name));
            }
            catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.TypeNotSupported || paramEx.Result == ParameterBinderResult.ValidationFailed)
            {
                _console.Error.WriteLine(string.Format(Strings.Command_Error_ParameterBind, paramEx.Message));
            }

            return 1;
        }
    }
}
