using System;
using System.Threading;
using Cocona;

namespace CoconaSample.Advanced.PreventMultipleInstances
{
    [PreventMultipleInstances]
    class Program : CoconaConsoleAppBase
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public void Run()
        {
            Console.WriteLine("Start long-running task...");

            while (!Context.CancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Canceled");
        }
    }
}
