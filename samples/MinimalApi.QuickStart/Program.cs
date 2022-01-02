using Cocona;

CoconaApp.Run(([Option('u', Description = "Print a name converted to upper-case.")] bool toUpperCase, [Argument(Description = "Your name")] string name) =>
    Console.WriteLine($"Hello {(toUpperCase ? name.ToUpper() : name)}!"));
