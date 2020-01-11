using Cocona;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace CoconaSampleApp.Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public void Echo([Option('n', Description = "Echo message to console.")]string name)
        {
            Console.WriteLine($"Hello from {name}");
        }

        public void Echo2([Argument]string name, [FromService]ILogger<Program> logger)
        {
            Console.WriteLine($"Hello from {name}");
        }
    }
}
