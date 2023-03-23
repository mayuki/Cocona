using Cocona;
using Microsoft.Extensions.Configuration;

namespace CoconaSample.InAction.AppConfiguration
{
    class Program
    {
        private bool _configValue1;
        private string _configValue2;

        // Example (on bash):
        //   dotnet run --
        //   DOTNET_ENVIROMENT=Development dotnet run --
        public Program(IConfiguration configuration)
        {
            // See. https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1
            _configValue1 = configuration.GetValue<bool>("ConfigValue1");
            _configValue2 = configuration.GetValue<string>("ConfigValue2");
        }

        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public void Run()
        {
            Console.WriteLine($"ConfigValue1: {_configValue1}");
            Console.WriteLine($"ConfigValue2: {_configValue2}");
        }
    }
}
