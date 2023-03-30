using Cocona;

namespace CoconaSample.InAction.CommandOptions;

class Program
{
    static void Main(string[] args)
    {
        CoconaApp.Run<Program>(args);
    }

    public void Number(int foo)
    {
        Console.WriteLine(foo);
    }

    public void Enum(User user)
    {
        Console.WriteLine(user);
    }

    public enum User
    {
        Alice, Karen, Other
    }

    // dotnet run -- booleantruebydefault --dryRun=false
    public void BooleanTrueByDefault(bool dryRun = true)
    {
        Console.WriteLine($"DryRun: {dryRun}");
    }

    // dotnet run -- array -I../path/to/foo -I../foo/bar/baz
    public void Array([Option('I')]string[] include)
    {
        foreach (var item in include)
        {
            Console.WriteLine(item);
        }
    }

    public void HiddenOptionInHelp(bool visible, [Hidden]bool hidden)
    {
        Console.WriteLine($"Visible={visible}; Hidden={hidden}");
    }

    public void HasDescription([Option(Description = "Description of the option")] int value, [Argument(Description = "Description of the argument")]string arg)
    {

    }
}