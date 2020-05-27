using System;
using Cocona;

namespace CoconaSample.Advanced.OptionLikeCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        [OptionLikeCommand("hello", new[] {'f'}, typeof(Program), nameof(Hello))]
        public void Execute()
        {
            Console.WriteLine("Execute");
        }

        private void Hello([Argument]string name)
        {
            Console.WriteLine($"Hello {name}!");
        }
    }
}
