using System;
using System.Threading;
using System.Threading.Tasks;
using Cocona.Filters;

namespace CoconaSample.Advanced.PreventMultipleInstances
{
    public class PreventMultipleInstancesAttribute : CommandFilterAttribute
    {
        private readonly Mutex _mutex = new Mutex(false, System.Reflection.Assembly.GetEntryAssembly().GetName().Name);

        public override ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
        {
            var isRunning = false;
            try
            {
                if (!_mutex.WaitOne(0, false))
                {
                    Console.Error.WriteLine("Error: The application is already running.");
                    return new ValueTask<int>(1);
                }

                isRunning = true;

                return next(ctx);
            }
            finally
            {
                if (isRunning)
                {
                    _mutex.ReleaseMutex();
                }
                _mutex.Dispose();
            }
        }
    }
}
