using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cocona
{
    public class CoconaApp
    {
        public static void Run<T>(string[] args)
        {
            RunAsync<T>(args).GetAwaiter().GetResult();
        }

        public static Task RunAsync<T>(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .UseCocona(args, new[] { typeof(T) })
                .Build()
                .RunAsync();
        }
    }
}
