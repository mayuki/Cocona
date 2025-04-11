extern alias WildCoco_v1_1_0;
using BenchmarkDotNet.Attributes;

namespace WildCoco.Benchmark.Performance;

[SimpleJob]
[MemoryDiagnoser]
public class ToCommandBenchmark
{
    [Benchmark(Description = "v1.1.0")]
    public void v1_1_0()
    {
        _ = WildCoco_v1_1_0::WildCoco.Command.WildCocoCommandProvider.ToCommandCase("FooBar");
    }

    [Benchmark(Description = "Current")]
    public void Current()
    {
        _ = global::WildCoco.Command.WildCocoCommandProvider.ToCommandCase("FooBar");
    }
}
