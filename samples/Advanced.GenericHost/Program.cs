using Microsoft.Extensions.Hosting;

namespace WildCocoSample.Advanced.GenericHost;

class Program
{
    static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder()
            .ConfigureWildCoco(args, new[] { typeof(Program) })
            .Build()
            .RunAsync();
    }

    public void Hello()
    {
        Console.WriteLine($"Hello Konnichiwa!");
    }
}
