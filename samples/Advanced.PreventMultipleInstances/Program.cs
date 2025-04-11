using WildCoco;

namespace WildCocoSample.Advanced.PreventMultipleInstances;

[PreventMultipleInstances]
class Program : WildCocoConsoleAppBase
{
    static void Main(string[] args)
    {
        WildCocoApp.Run<Program>(args);
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Start long-running task...");

        while (!Context.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(100);
        }
        await Task.Run(() => Task.Delay(100));

        Console.WriteLine("Canceled");
    }
}
