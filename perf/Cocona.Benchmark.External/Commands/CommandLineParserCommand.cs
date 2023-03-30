namespace Cocona.Benchmark.External.Commands;

public class CommandLineParserCommand
{
    [global::CommandLine.Option('s', "str")]
    public string? StrOption { get; set; }

    [global::CommandLine.Option('i', "int")]
    public int IntOption { get; set; }

    [global::CommandLine.Option('b', "bool")]
    public bool BoolOption { get; set; }

    public void Execute()
    {
    }
}