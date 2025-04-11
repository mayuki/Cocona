extern alias WildCoco_v1_1_0;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using WildCoco.Command;

namespace WildCoco.Benchmark.Performance;

[SimpleJob(RunStrategy.ColdStart, launchCount: 20)]
[SimpleJob]
[MemoryDiagnoser]
public class CommandProviderBenchmark
{
    [Benchmark]
    public void CommandProvider_v1_1_0()
    {
        var provider = new WildCoco_v1_1_0::WildCoco.Command.WildCocoCommandProvider(
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
        var provider = new WildCoco_v1_1_0::WildCoco.Command.BuiltIn.WildCocoBuiltInCommandProvider(new WildCoco_v1_1_0::WildCoco.Command.WildCocoCommandProvider(
            new[] { typeof(TestCommand_v1_1_0) },
            treatPublicMethodsAsCommands: true,
            enableConvertCommandNameToLowerCase: true,
            enableConvertOptionNameToLowerCase: true
        ));

        var commands = provider.GetCommandCollection();
    }

    class TestCommand_v1_1_0
    {
        public void Hello([WildCoco_v1_1_0::WildCoco.Option('b')]bool boolOption, [WildCoco_v1_1_0::WildCoco.Option('s')]string strOption, [WildCoco_v1_1_0::WildCoco.Argument]string arg0)
        { }
    }

    [Benchmark]
    public void CommandProvider_Current()
    {
        var provider = new global::WildCoco.Command.WildCocoCommandProvider(
            new[] { typeof(TestCommand_Current) },
            options: CommandProviderOptions.TreatPublicMethodAsCommands | CommandProviderOptions.CommandNameToLowerCase | CommandProviderOptions.OptionNameToLowerCase | CommandProviderOptions.ArgumentNameToLowerCase
        );

        var commands = provider.GetCommandCollection();
    }
    [Benchmark]
    public void BuiltInCommandProvider_Current()
    {
        var provider = new global::WildCoco.Command.BuiltIn.WildCocoBuiltInCommandProvider(new global::WildCoco.Command.WildCocoCommandProvider(
            new[] { typeof(TestCommand_Current) },
            options: CommandProviderOptions.TreatPublicMethodAsCommands | CommandProviderOptions.CommandNameToLowerCase | CommandProviderOptions.OptionNameToLowerCase | CommandProviderOptions.ArgumentNameToLowerCase
        ), enableShellCompletionSupport:true);

        var commands = provider.GetCommandCollection();
    }

    class TestCommand_Current
    {
        public void Hello([global::WildCoco.Option('b')]bool boolOption, [global::WildCoco.Option('s')]string strOption, [global::WildCoco.Argument]string arg0)
        { }
    }
}
