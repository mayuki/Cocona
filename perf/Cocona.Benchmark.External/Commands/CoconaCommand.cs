namespace Cocona.Benchmark.External.Commands
{
    public class CoconaCommand
    {
        public void Execute(
            [global::Cocona.Option("str", new []{'s'})]
            string? strOption,
            [global::Cocona.Option("int", new []{'i'})]
            int intOption,
            [global::Cocona.Option("bool", new []{'b'})]
            bool boolOption)
        {
        }
    }
}
