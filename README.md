# ![Cocona](docs/assets/logo.svg)
Micro-framework for .NET **Co**re **con**sole **a**pplication. Cocona makes it easy and blazing-fast to build console applications on .NET Core.🚀

[![Build Status](https://dev.azure.com/misuzilla/Cocona/_apis/build/status/Cocona?branchName=master)](https://dev.azure.com/misuzilla/Cocona/_build/latest?definitionId=18&branchName=master) [![NuGet Package: Cocona](https://img.shields.io/nuget/vpre/Cocona)](https://www.nuget.org/packages/Cocona)

### ⏱ Create a console application with Cocona in seconds.
![](docs/assets/intro-in-seconds.gif)

## Feature
- 🚀 **Make it easy to build console applications on .NET Core.**
    - `public` method as a command ™ by default
    - Provides ASP.NET Core MVC-like development experience to console application development.
- ✨ **Command-line option semantics like UNIX tools standard. (`getopt`/`getopt_long` like options)**
    - Your app can handle both `-rf /` and `-r -f /` :-)
    - Support single command and multiple commands style
        - `myapp --foo --bar -n arg0 "arg1"` (e.g. `dir`, `cp`, `ls` ...)
        - `myapp server -m "Hello world!"` (e.g. `dotnet`, `git`, `kubectl` ...)
- ❓ **Built-in help documentation support.**
    - You want to see a help message; you type `-h` or `--help`.
    - Built-in similar commands suggestion
- 🛠 **Highly modulable/customizable CLI framework.**
    - Cocona built on top of `Microsoft.Extensions.*` framework. Cocona natively supports Logging, DI, Configuration and ConsoleLifetime.

[You can find sample code for various features.](samples)

## Table of contents
- [Installing](#installing)
- [Getting Started](#getting-started)
- [Command-line handling basics](#command-line-handling-basics)
    - [Command](#command)
    - [Options](#options)
    - Arguments
    - Exit code
- Sub-commands
- Cocona in action
    - Logging
    - Dependency Injection
    - Configuration
    - Shutdown
    - Middleware
- Advanced
    - Help customization
    - Customize providers
- [License](#license)

## Installing
Install NuGet package from NuGet.org

```sh
$ dotnet add package Cocona
```
```powershell
PS> Install-Package Cocona
```

## Getting Started

```csharp
using Cocona;
class Program
{
    static void Main(string[] args)
    {
        // Parse command-line and dispatch command.
        CoconaApp.Run<Program>(args);
    }

    // public method as a command ™
    public void Hello(string name)
    {
        Console.WriteLine($"Hello {name}");
    }
}
```

### Try to run!
```sh
$ dotnet run
Usage: ConsoleAppSample [--name <String>]

Options:
  --name <String>    (Required)
  -h, --help    Show help message
  --version     Show version

$ dotnet run -- --name Cocona
Hello Cocona
```

### Extra: Publish the application as a single-file executable
If your application runs on .NET Core 3.0 or later, you can publish the app as a single-file executable. (see. [What's new in .NET Core 3.0](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-core-3-0#single-file-executables))
```sh
PS> dotnet publish -r win-x64 -p:PublishSingleFile=true
PS> app.exe --name Cocona

$ dotnet publish -r linux-x64 -p:PublishSingleFile=true
$ ./app --name Cocona
```

## Command-line handling basics
### Command
#### Public method as a command ™
By default, Cocona treats `public` methods as commands.

If an app has one public method, Cocona calls it on startup. If there are two or more, they are treated as subcommands. (see also [Subcommands](#subcommands))

```csharp
// Treats a method name as a command name. (Below method is named `command`)
public void Command() { ... }

// Specify a command name using CommandAttribute.
[Command("commandname")]
public void Command() { ... }

// Cocona will ignore this method.
[Ignore]
public void Ignored() { ... }
```

If you want to specify method as a command manually, set `false` to `TreatPublicMethodsAsCommands` option at startup.
```csharp
CoconaApp.Run<Program>(args, options =>
{
    // If the option value is `false`, All command methods requires `CommandAttribute`.
    options.TreatPublicMethodsAsCommands = false;
});
```
### Options
Cocona exposes method parameters as command-line options (also known as flags).

```csharp
// This command accepts `--name <string>` and `--hey` options.
public void Hello(string name, bool hey) { ... }
```

If the parameters are [optional argument](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/named-and-optional-arguments#optional-arguments), Cocona will treat those as optional command options. (That is, the parameters are treated as **required option** by default).

```csharp
// `--name "default user"` is specified implicity.
public void Hello(string name = "default user") { ... }
```

Do you want to use short-name option `-f` instead of `--force`. You can specify short-name to an option using `OptionAttribute`.

```csharp
public void Remove([Option('f')]bool force) { ... }
```

## License
MIT License
```
Copyright © 2020-present Mayuki Sawatari <mayuki@misuzilla.org>
```