using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Cocona.Benchmark.External.Commands;

[CliFx.Attributes.Command]
public class CliFxCommand : CliFx.ICommand
{
    [CommandOption("str", 's')]
    public string? StrOption { get; set; }

    [CommandOption("int", 'i')]
    public int IntOption { get; set; }

    [CommandOption("bool", 'b')]
    public bool BoolOption { get; set; }
    
    public ValueTask ExecuteAsync(IConsole console) => ValueTask.CompletedTask;
}
