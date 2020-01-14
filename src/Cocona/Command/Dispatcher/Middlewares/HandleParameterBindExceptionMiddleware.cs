using Cocona.Command.Binder;
using Cocona.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocona.Command.Dispatcher.Middlewares
{
    public class HandleParameterBindExceptionMiddleware : CommandDispatcherMiddleware
    {
        public HandleParameterBindExceptionMiddleware(CommandDispatchDelegate next) : base(next)
        {
        }

        public override async ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
        {
            try
            {
                return await Next(ctx);
            }
            catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.InsufficientArgument)
            {
                Console.Error.WriteLine($"Error: Argument '{paramEx.Argument!.Name}' is required.");
            }
            catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.InsufficientOption)
            {
                Console.Error.WriteLine($"Error: Option '--{paramEx.Option!.Name}' is required.");
            }
            catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.InsufficientOptionValue)
            {
                Console.Error.WriteLine($"Error: Option '--{paramEx.Option!.Name}' requires a value.");
            }
            catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.TypeNotSupported)
            {
                Console.Error.WriteLine($"Error: {paramEx.Message}");
            }

            return 1;
        }
    }
}
