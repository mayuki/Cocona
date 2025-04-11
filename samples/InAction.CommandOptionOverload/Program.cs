using WildCoco;
using WildCoco;

namespace WildCocoSample.InAction.CommandOptionOverload;

class Program
{
    static void Main(string[] args)
    {
        WildCocoApp.Run<Program>(args);
    }

    // dotnet run -- --mode foobar
    // ./WildCocoSample.InAction.CommandOptionOverload --mode foobar
    public void Run(string mode)
    {
        Console.WriteLine($"Run mode={mode}");
    }

    // dotnet run -- --mode extra
    // ./WildCocoSample.InAction.CommandOptionOverload --mode extra
    [CommandOverload("Run", "mode", "extra")]
    public void RunModeExtra(string mode)
    {
        Console.WriteLine($"Run mode={mode} | Option Overloaded!");
    }

}
