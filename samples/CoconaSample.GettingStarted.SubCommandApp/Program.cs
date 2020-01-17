using System;
using Cocona;

namespace CoconaSample.GettingStarted.SubCommandApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
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
}
