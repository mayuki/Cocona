using System;
using Cocona;
using Cocona.Application;
using Cocona.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = CoconaApp.CreateBuilder();
builder.Services.AddSingleton<MyService>(new MyService("Karen"));
builder.Configuration.AddJsonFile("appsettings.custom.json", optional: true);

var app = builder.Build();

// Add a command and set its alias.
app.AddCommand("hello", (string name) => Console.WriteLine($"Hello {name}"))
    .WithDescription("Say hello")
    .WithAliases("hey", "konnichiwa");

// Add a command and use the context to cancel with Ctrl+C.
app.AddCommand("long-running", async (CoconaAppContext ctx) =>
{
    Console.WriteLine($"Running...");
    await Task.Delay(TimeSpan.FromSeconds(30), ctx.CancellationToken);
    Console.WriteLine($"Done.");
});

// Add a hidden command.
app.AddCommand("secret-command", () => Console.WriteLine(":-)"))
    .WithMetadata(new HiddenAttribute());

// Add a command and use Dependency Injection for the command parameter.
app.AddCommand("with-di", (MyService myService) => Console.WriteLine($"Hello {myService.GetName()}"));

// Add a sub-command.
app.AddSubCommand("admin", x =>
{
    x.AddCommand("start-server", () => Console.WriteLine("Starting the server..."));
    x.AddCommand("stop-server", () => Console.WriteLine("Stopping the server..."));
    x.UseFilter(new RequirePrivilege());
    x.AddCommand("delete-server", () => Console.WriteLine("Deleting the server..."));
});

// Add a command with command filters.
app.AddCommand("with-filter", () => Console.WriteLine("Hello Konnichiwa!"))
    .WithFilter(async (ctx, next) =>
    {
        // Inline CommandFilter
        Console.WriteLine("Before");
        try
        {
            return await next(ctx);
        }
        finally
        {
            Console.WriteLine("End");
        }
    });

// Add a command filter and apply it to commands after this call.
app.UseFilter(new LoggingFilter(app.Services.GetRequiredService<ILogger<LoggingFilter>>()));
app.AddCommand("with-global-filter", () => Console.WriteLine("Hello Konnichiwa!"));

app.Run();

record MyService(string Name)
{
    public string GetName() => Name;
}

class LoggingFilter : CommandFilterAttribute
{
    private readonly ILogger _logger;

    public LoggingFilter(ILogger<LoggingFilter> logger)
    {
        _logger = logger;
    }
    public override async ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
    {
        _logger.LogInformation($"Before {ctx.Command.Name}");
        try
        {
            return await next(ctx);
        }
        finally
        {
            _logger.LogInformation($"End {ctx.Command.Name}");
        }
    }
}

class RequirePrivilege : CommandFilterAttribute
{
    public override ValueTask<int> OnCommandExecutionAsync(CoconaCommandExecutingContext ctx, CommandExecutionDelegate next)
    {
        if (Environment.UserName != "Administrator" || Environment.UserName != "root")
        {
            throw new CommandExitedException("Error: Permission denied.", 1);
        }
        return next(ctx);
    }
}
