using Cocona;

namespace CoconaSample.Advanced.OptionLikeCommand;

class Program
{
    static void Main(string[] args)
    {
        CoconaApp.Run<Program>(args);
    }

    [OptionLikeCommand("hello", new[] {'f'}, typeof(Program), nameof(Hello))]
    [OptionLikeCommand("bye", new[] {'b'}, typeof(Program), nameof(Bye), "Show bye message")]
    public void Execute()
    {
        Console.WriteLine("Execute");
    }

    private void Hello([Argument]string name)
    {
        Console.WriteLine($"Hello {name}!");
    }

    private void Bye([Argument]string name)
    {
        Console.WriteLine($"Bye {name}!");
    }
}