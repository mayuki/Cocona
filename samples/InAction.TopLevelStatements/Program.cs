using System;
using Cocona;

var app = CoconaApp.Create();

#if NET6_0_OR_GREATER
app.AddCommand([Command("hello")]([Argument] string name) => Console.WriteLine($"Hello {name}!"));
app.AddCommand([Command("konnichiwa")]([Argument] string name) => Console.WriteLine($"Konnichiwa {name}!"));
#else
app.AddCommand((Action<string>)Hello);
app.AddCommand((Action<string>)Konnichiwa);

[Command("hello")]
static void Hello([Argument] string name)
    => Console.WriteLine($"Hello {name}!");

[Command("konnichiwa")]
static void Konnichiwa([Argument] string name)
    => Console.WriteLine($"Konnichiwa {name}!");
#endif

app.Run();
