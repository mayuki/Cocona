using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.ConfigureWildCoco(args, new[] { typeof(Commands) });
                
var host = builder.Build();

await host.RunAsync();

public class Commands
{
    public void Hello() => Console.WriteLine($"Hello Konnichiwa!");
}
