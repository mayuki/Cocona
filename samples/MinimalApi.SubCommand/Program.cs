using Cocona;

var app = CoconaApp.Create();

app.AddSubCommand("client", x =>
{
    x.AddCommand("connect", () => Console.WriteLine("Connecting to the server..."));
    x.AddCommand("disconnect", () => Console.WriteLine("Disconnecting from the server..."));
});
app.AddSubCommand("server", x =>
{
    x.AddCommand("start", () => Console.WriteLine("Starting the server.."));
    x.AddCommand("stop", () => Console.WriteLine("Stopping the server..."));
});

app.Run();
