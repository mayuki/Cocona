namespace Cocona.Benchmark.External.Commands;

public class McMasterCommand
{
    [McMaster.Extensions.CommandLineUtils.Option("--str|-s")]
    public string? StrOption { get; set; }

    [McMaster.Extensions.CommandLineUtils.Option("--int|-i")]
    public int IntOption { get; set; }

    [McMaster.Extensions.CommandLineUtils.Option("--bool|-b")]
    public bool BoolOption { get; set; }

    public int OnExecute() => 0;
}