using Cocona;
using System;

namespace CoconaSample.GettingStarted.MinimalApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public void Hello(bool toUppperCase, [Argument]string name)
        {
            Console.WriteLine($"Hello {(toUppperCase ? name.ToUpper() : name)}");
        }
    }
}
