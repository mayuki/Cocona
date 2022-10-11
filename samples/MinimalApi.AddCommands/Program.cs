using Cocona;
CoconaApp
.CreateBuilder()
.Build()
.AddCommand("hello", (string name) => Console.WriteLine($"Hello {name}!"))
.AddCommand("bye", (string name) => Console.WriteLine($"Bye {name}!"))
.Run();
