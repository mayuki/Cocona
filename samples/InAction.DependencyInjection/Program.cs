using System;
using System.Threading.Tasks;
using Cocona;
using Microsoft.Extensions.Logging;

namespace CoconaSample.InAction.DependencyInjection
{
    class Program
    {
        public Program(ILogger<Program> logger)
        {
            logger.LogInformation("Create Instance");
        }

        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public void Hello([FromService]ILogger<Program> logger)
        {
            logger.LogInformation($"Hello Konnichiwa");
        }
    }
}
