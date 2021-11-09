using Cocona;

var app = CoconaApp.Create();

app.AddCommand((bool toUpperCase, [Argument] string name) =>
{
    Console.WriteLine($"Hello {(toUpperCase ? name.ToUpper() : name)}");
});

await app.RunAsync();
