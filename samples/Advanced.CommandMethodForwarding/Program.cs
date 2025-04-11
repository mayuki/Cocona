using WildCoco;
using WildCoco;

namespace WildCocoSample.Advanced.CommandMethodForwarding;

class Program
{
    static void Main(string[] args)
    {
        WildCocoApp.Run<Program>(args);
    }

    [CommandMethodForwardedTo(typeof(Program), nameof(Program.Hey))]
    public void Hello()
        => throw new NotImplementedException();

    public void Hey([Argument]string name)
        => Console.WriteLine($"Hello {name}");

    [CommandMethodForwardedTo(typeof(WildCoco.Command.BuiltIn.BuiltInOptionLikeCommands), nameof(WildCoco.Command.BuiltIn.BuiltInOptionLikeCommands.ShowHelp))]
    public void MyHelp()
        => throw new NotSupportedException();
}
