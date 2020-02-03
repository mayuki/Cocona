using System;
using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Cocona.Benchmark.Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run(typeof(Program).Assembly, DefaultConfig.Instance);
            //BenchmarkRunner.Run<ToCommandBenchmark>(DefaultConfig.Instance);
            BenchmarkRunner.Run<CommandProviderBenchmark>(DefaultConfig.Instance);
        }
    }
}
