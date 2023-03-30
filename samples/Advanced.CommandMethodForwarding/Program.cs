using Cocona;

namespace CoconaSample.Advanced.CommandMethodForwarding
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        [CommandMethodForwardedTo(typeof(Program), nameof(Program.Hey))]
        public void Hello()
            => throw new NotImplementedException();

        public void Hey([Argument]string name)
            => Console.WriteLine($"Hello {name}");

        [CommandMethodForwardedTo(typeof(Cocona.Command.BuiltIn.BuiltInOptionLikeCommands), nameof(Cocona.Command.BuiltIn.BuiltInOptionLikeCommands.ShowHelp))]
        public void MyHelp()
            => throw new NotSupportedException();
    }
}
