using System.Text.Json;
using Cocona;
using Cocona.Command.Binder;
using Microsoft.Extensions.DependencyInjection;

namespace CoconaSample.Advanced.JsonValueConverter;

class Program
{
    static async Task Main(string[] args)
    {
        await CoconaApp.CreateHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<ICoconaValueConverter, JsonValueConverter>();
            })
            .RunAsync<Program>(args);
    }

    // dotnet run -- "{\"Name\":\"Alice\",\"Age\":18}"
    public void Hello([Argument]User user)
    {
        Console.WriteLine($"User: {user.Name} ({user.Age})");
    }

    public class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}

class JsonValueConverter : ICoconaValueConverter
{
    public object ConvertTo(Type t, string value)
    {
        return JsonSerializer.Deserialize(value, t);
    }
}