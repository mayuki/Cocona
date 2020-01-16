using System;
using Cocona;

namespace CoconaSample.InAction.ManyArguments
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        // Example:
        //   $ ./copy file1 file2 file3 /path/to/dest/
        //   Source: file1, file2, file3
        //   Destination: /path/to/dest/
        public void Copy([Argument]string[] srcs, [Argument]string dest)
        {
            Console.WriteLine($"Source: {string.Join(", ", srcs)}");
            Console.WriteLine($"Destination: {dest}");
        }
    }
}
