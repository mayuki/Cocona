using Cocona;

var app = CoconaApp.Create();

app.AddCommand("hello", ([Option('u', Description = "Print a name converted to upper-case.")] bool toUpperCase, [Argument(Description = "Your name")] string name) =>
    {
        Console.WriteLine($"Hello {(toUpperCase ? name.ToUpper() : name)}!");
    })
    .WithDescription("Say hello");

app.AddCommand("goodbye", ([Option('u', Description = "Print a name converted to upper-case.")] bool toUpperCase, [Argument(Description = "Your name")] string name) =>
    {
        Console.WriteLine($"Goodbye {(toUpperCase ? name.ToUpper() : name)}!");
    })
    .WithDescription("Say goodbye");

await app.RunAsync();
