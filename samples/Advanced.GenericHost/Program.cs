using Microsoft.Extensions.Hosting;

namespace CoconaSample.Advanced.GenericHost;

class Program
{
    static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder()
            .ConfigureCocona(args, new[] { typeof(Program) })
            .Build()
            .RunAsync();
    }

    public void Hello()
    {
        Console.WriteLine($"Hello Konnichiwa!");
    }
}