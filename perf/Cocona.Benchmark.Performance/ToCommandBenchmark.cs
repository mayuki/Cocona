extern alias Cocona_v1_1_0;
using BenchmarkDotNet.Attributes;

namespace Cocona.Benchmark.Performance
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class ToCommandBenchmark
    {
        [Benchmark(Description = "v1.1.0")]
        public void v1_1_0()
        {
            _ = Cocona_v1_1_0::Cocona.Command.CoconaCommandProvider.ToCommandCase("FooBar");
        }

        [Benchmark(Description = "Current")]
        public void Current()
        {
            _ = global::Cocona.Command.CoconaCommandProvider.ToCommandCase("FooBar");
        }
    }
}
