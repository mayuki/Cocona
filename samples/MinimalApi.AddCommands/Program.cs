using Cocona;
CoconaApp
.CreateBuilder()
.Build()
.AddCommands
(
    ("hello", (string name) => Console.WriteLine($"Hello {name}!")),
    ("bye", (string name) => Console.WriteLine($"Bye {name}!"))
)
.AddCommands(("foo", () => Console.WriteLine("Bar")))
.AsCoconaApp()
.Run();
