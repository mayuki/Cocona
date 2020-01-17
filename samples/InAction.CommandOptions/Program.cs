using System;
using Cocona;

namespace CoconaSample.InAction.CommandOptions
{
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

        public void BooleanByDefault(bool dryRun = true)
        {
            Console.WriteLine($"DryRun: {dryRun}");
        }
    }
}
