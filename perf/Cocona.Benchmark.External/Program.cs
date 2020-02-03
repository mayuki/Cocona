// This benchmark project is based on CliFx.Benchmarks.
// https://github.com/Tyrrrz/CliFx/tree/master/CliFx.Benchmarks/
using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Cocona.Benchmark.External
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(Program).Assembly, DefaultConfig.Instance.With(ConfigOptions.DisableOptimizationsValidator));
        }
    }
}
