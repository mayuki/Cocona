using WildCoco;

namespace WildCocoSample.InAction.HandleShutdownSignal;

class Program : WildCocoConsoleAppBase
{
    static void Main(string[] args)
    {
        WildCocoApp.Run<Program>(args);
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Press Ctrl+C to shutdown the application.");
        Console.WriteLine("Start long-running task...");

        while (!Context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(100);
        }

        Console.WriteLine("Shutting down...");
    }
}
