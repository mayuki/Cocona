// This benchmark project is based on CliFx.Benchmarks.
// https://github.com/Tyrrrz/CliFx/tree/master/CliFx.Benchmarks/

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using CliFx;
using Cocona.Benchmark.External.Commands;
using CommandLine;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;

namespace Cocona.Benchmark.External
{
    [SimpleJob]
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class Benchmark
    {
        private static readonly string[] Arguments = { "--str", "hello world", "-i", "13", "-b" };

        [Benchmark(Description = "Cocona.Lite", Baseline = true)]
        public async Task ExecuteWithCoconaLite() =>
            await Cocona.CoconaLiteApp.RunAsync<CoconaCommand>(Arguments);

        [Benchmark(Description = "Cocona")]
        public async ValueTask ExecuteWithCocona() =>
            await Cocona.CoconaApp.RunAsync<CoconaCommand>(Arguments);

        [Benchmark(Description = "ConsoleAppFramework")]
        public async ValueTask ExecuteWithConsoleAppFramework() =>
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<ConsoleAppFrameworkCommand>(Arguments);

        [Benchmark(Description = "CliFx")]
        public async ValueTask<int> ExecuteWithCliFx() =>
            await new CliApplicationBuilder().AddCommand(typeof(CliFxCommand)).Build().RunAsync(Arguments);

        [Benchmark(Description = "System.CommandLine")]
        public async Task<int> ExecuteWithSystemCommandLine() =>
            await new SystemCommandLineCommand().ExecuteAsync(Arguments);

        [Benchmark(Description = "McMaster.Extensions.CommandLineUtils")]
        public int ExecuteWithMcMaster() =>
            McMaster.Extensions.CommandLineUtils.CommandLineApplication.Execute<McMasterCommand>(Arguments);

        [Benchmark(Description = "CommandLineParser")]
        public void ExecuteWithCommandLineParser() =>
            new Parser()
                .ParseArguments(Arguments, typeof(CommandLineParserCommand))
                .WithParsed<CommandLineParserCommand>(c => c.Execute());

        [Benchmark(Description = "PowerArgs")]
        public void ExecuteWithPowerArgs() =>
            PowerArgs.Args.InvokeMain<PowerArgsCommand>(Arguments);

        [Benchmark(Description = "Clipr")]
        public void ExecuteWithClipr() =>
            clipr.CliParser.Parse<CliprCommand>(Arguments).Execute();
    }
}
