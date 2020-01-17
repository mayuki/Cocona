using System;
using System.Threading.Tasks;
using Cocona;
using Microsoft.Extensions.Hosting;

namespace CoconaSample.Advanced.GenericHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                .UseCocona(args, new[] { typeof(Program) })
                .Build()
                .RunAsync();
        }

        public void Hello()
        {
            Console.WriteLine($"Hello Konnichiwa!");
        }
    }
}
