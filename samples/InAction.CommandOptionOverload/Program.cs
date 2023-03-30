using Cocona;

namespace CoconaSample.InAction.CommandOptionOverload;

class Program
{
    static void Main(string[] args)
    {
        CoconaApp.Run<Program>(args);
    }

    // dotnet run -- --mode foobar
    // ./CoconaSample.InAction.CommandOptionOverload --mode foobar
    public void Run(string mode)
    {
        Console.WriteLine($"Run mode={mode}");
    }

    // dotnet run -- --mode extra
    // ./CoconaSample.InAction.CommandOptionOverload --mode extra
    [CommandOverload("Run", "mode", "extra")]
    public void RunModeExtra(string mode)
    {
        Console.WriteLine($"Run mode={mode} | Option Overloaded!");
    }

}