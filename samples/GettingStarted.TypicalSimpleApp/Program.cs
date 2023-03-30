using Cocona;

namespace CoconaSample.GettingStarted.TypicalSimpleApp;

class Program
{
    static void Main(string[] args)
    {
        CoconaApp.Run<Program>(args);
    }

    [Command(Description = "This is a sample application")]
    public void Hello([Option('u', Description = "Print a name converted to upper-case.")]bool toUpperCase, [Argument(Description = "Your name")]string name)
    {
        Console.WriteLine($"Hello {(toUpperCase ? name.ToUpper() : name)}!");
    }
}