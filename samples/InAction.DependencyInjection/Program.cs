using Cocona;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoconaSample.InAction.DependencyInjection;

class Program
{
    public Program(ILogger<Program> logger)
    {
        logger.LogInformation("Create Instance");
    }

    static void Main(string[] args)
    {
        CoconaApp.CreateHostBuilder()
            .ConfigureLogging(logging =>
            {
                logging.AddDebug();
            })
            .ConfigureServices(services =>
            {
                services.AddTransient<MyService>();
            })
            .Run<Program>(args);
    }

    public void Hello([FromService]MyService myService)
    {
        myService.Hello("Hello Konnichiwa!");
    }
}

class MyService
{
    private readonly ILogger _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public void Hello(string message)
    {
        _logger.LogInformation(message);
    }
}