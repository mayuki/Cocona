extern alias Cocona_v1_1_0;

using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace Cocona.Benchmark.Performance
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 20)]
    [SimpleJob]
    [MemoryDiagnoser]
    public class CommandProviderBenchmark
    {
        [Benchmark]
        public void CommandProvider_v1_1_0()
        {
            var provider = new Cocona_v1_1_0::Cocona.Command.CoconaCommandProvider(
                new [] { typeof(TestCommand_v1_1_0) },
                treatPublicMethodsAsCommands:true,
                enableConvertCommandNameToLowerCase:true,
                enableConvertOptionNameToLowerCase:true
            );

            var commands = provider.GetCommandCollection();
        }
        [Benchmark]
        public void BuiltInCommandProvider_v1_1_0()
        {
            var provider = new Cocona_v1_1_0::Cocona.Command.BuiltIn.CoconaBuiltInCommandProvider(new Cocona_v1_1_0::Cocona.Command.CoconaCommandProvider(
                new[] { typeof(TestCommand_v1_1_0) },
                treatPublicMethodsAsCommands: true,
                enableConvertCommandNameToLowerCase: true,
                enableConvertOptionNameToLowerCase: true
            ));

            var commands = provider.GetCommandCollection();
        }

        class TestCommand_v1_1_0
        {
            public void Hello([Cocona_v1_1_0::Cocona.Option('b')]bool boolOption, [Cocona_v1_1_0::Cocona.Option('s')]string strOption, [Cocona_v1_1_0::Cocona.Argument]string arg0)
            { }
        }

        [Benchmark]
        public void CommandProvider_Current()
        {
            var provider = new global::Cocona.Command.CoconaCommandProvider(
                new[] { typeof(TestCommand_Current) },
                treatPublicMethodsAsCommands: true,
                enableConvertCommandNameToLowerCase: true,
                enableConvertOptionNameToLowerCase: true
            );

            var commands = provider.GetCommandCollection();
        }
        [Benchmark]
        public void BuiltInCommandProvider_Current()
        {
            var provider = new global::Cocona.Command.BuiltIn.CoconaBuiltInCommandProvider(new global::Cocona.Command.CoconaCommandProvider(
                new[] { typeof(TestCommand_Current) },
                treatPublicMethodsAsCommands: true,
                enableConvertCommandNameToLowerCase: true,
                enableConvertOptionNameToLowerCase: true,
                enableConvertArgumentNameToLowerCase: true
            ), enableShellCompletionSupport:true);

            var commands = provider.GetCommandCollection();
        }

        class TestCommand_Current
        {
            public void Hello([global::Cocona.Option('b')]bool boolOption, [global::Cocona.Option('s')]string strOption, [global::Cocona.Argument]string arg0)
            { }
        }
    }
}
