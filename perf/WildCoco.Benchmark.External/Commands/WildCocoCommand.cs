namespace WildCoco.Benchmark.External.Commands;

public class WildCocoCommand
{
    public void Execute(
        [global::WildCoco.Option("str", new []{'s'})]
        string? strOption,
        [global::WildCoco.Option("int", new []{'i'})]
        int intOption,
        [global::WildCoco.Option("bool", new []{'b'})]
        bool boolOption)
    {
    }
}
