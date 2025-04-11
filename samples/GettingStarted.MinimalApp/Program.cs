using WildCoco;
using WildCoco;

namespace WildCocoSample.GettingStarted.MinimalApp;

class Program
{
    static void Main(string[] args)
    {
        WildCocoApp.Run<Program>(args);
    }

    public void Hello(bool toUpperCase, [Argument]string name)
    {
        Console.WriteLine($"Hello {(toUpperCase ? name.ToUpper() : name)}");
    }
}
