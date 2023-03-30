using Cocona;

namespace CoconaSample.GettingStarted.SubCommandApp;

[HasSubCommands(typeof(SubCommands), Description = "Nested sub-commands")]
class Program
{
    static void Main(string[] args)
    {
        CoconaApp.Run<Program>(args, options =>
        {
            options.EnableShellCompletionSupport = true;
        });
    }

    [Command(Description = "Say hello")]
    public void Hello([Option('u', Description = "Print a name converted to upper-case.")]bool toUpperCase, [Argument(Description = "Your name")]string name)
    {
        Console.WriteLine($"Hello {(toUpperCase ? name.ToUpper() : name)}!");
    }

    [Command(Description = "Say goodbye")]
    public void Bye([Option('l', Description = "Print a name converted to lower-case.")]bool toLowerCase, [Argument(Description = "Your name")]string name)
    {
        Console.WriteLine($"Goodbye {(toLowerCase ? name.ToLower() : name)}!");
    }
}

// ./myapp sub-commands [command]
[HasSubCommands(typeof(SubSubCommands))]
class SubCommands
{
    public enum Member
    {
        Alice,
        Karen,
    }

    public void Konnichiwa(Member member)
    {
        Console.WriteLine($"Konnichiwa! {member}");
    }

    public void Hello()
    {
        Console.WriteLine("Hello!");
    }
}

// ./myapp sub-commands sub-sub-commands [command]
class SubSubCommands
{
    public void Foobar()
    {
        Console.WriteLine("Foobar!");
    }

    [PrimaryCommand]
    public void Primary(string value)
    {
        Console.WriteLine($"value={value}");
    }
}