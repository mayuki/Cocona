using Cocona.Filters;

namespace CoconaSample.Advanced.PreventMultipleInstances;

public class PreventMultipleInstancesAttribute : CommandFilterAttribute
{
    private readonly Mutex _mutex;
    private readonly Thread _mutexAcquireThread;
    private readonly ManualResetEventSlim _waitForExit;

    public PreventMultipleInstancesAttribute()
    {
        _mutex = new Mutex(false, System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
        _mutexAcquireThread = new Thread(AcquireAndHoldMutexInThread);
        _waitForExit = new ManualResetEventSlim();
    }

    public override async ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
    {
        var tcs = new TaskCompletionSource<bool>();
        _mutexAcquireThread.Start(tcs);

        try
        {
            var acquired = await tcs.Task;
            if (!acquired)
            {
                Console.Error.WriteLine("Error: The application is already running.");
                return 1;
            }

            return await next(ctx);
        }
        finally
        {
            _waitForExit.Set();
            _mutexAcquireThread.Join();
        }
    }

    private void AcquireAndHoldMutexInThread(object state)
    {
        var tcs = (TaskCompletionSource<bool>)state;
        var acquired = false;
        try
        {
            acquired = _mutex.WaitOne(0, false);
            tcs.SetResult(acquired);

            if (acquired)
            {
                _waitForExit.Wait();
            }
        }
        finally
        {
            if (acquired)
            {
                _mutex.ReleaseMutex();
            }

            _mutex.Dispose();
        }
    }
}