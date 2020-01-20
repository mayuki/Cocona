using System;
using Cocona;

namespace CoconaSample.InAction.MultipleCommandTypes
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run(args, new[] { typeof(Program), typeof(GreeterCommands) });
        }

        public void Foo()
        { }
        public void Bar()
        { }
        public void Baz()
        { }
    }

    class GreeterCommands
    {
        public void Hello()
        { }

        public void Konnichiwa()
        { }
    }
}
