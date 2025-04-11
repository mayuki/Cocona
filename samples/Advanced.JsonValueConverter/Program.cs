using System.Text.Json;
using WildCoco;
using WildCoco.Command.Binder;
using WildCoco;
using WildCoco.Command.Binder;
using Microsoft.Extensions.DependencyInjection;

namespace WildCocoSample.Advanced.JsonValueConverter;

class Program
{
    static async Task Main(string[] args)
    {
        await WildCocoApp.CreateHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IWildCocoValueConverter, JsonValueConverter>();
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

class JsonValueConverter : IWildCocoValueConverter
{
    public object ConvertTo(Type t, string value)
    {
        return JsonSerializer.Deserialize(value, t);
    }
}
