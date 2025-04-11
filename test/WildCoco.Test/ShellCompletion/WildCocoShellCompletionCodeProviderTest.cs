using WildCoco.Command;
using WildCoco.ShellCompletion;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Generators;
using WildCoco.Command;
using WildCoco.ShellCompletion;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Generators;
using Microsoft.Extensions.DependencyInjection;

namespace WildCoco.Test.ShellCompletion;

public class WildCocoShellCompletionCodeProviderTest
{
    [Fact]
    public void BuildGenerator()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IWildCocoShellCompletionCodeGenerator, TestCodeGenerator>();
        services.AddSingleton<IWildCocoShellCompletionCodeGenerator, Test2CodeGenerator>();
        services.AddSingleton<IWildCocoShellCompletionCodeProvider, WildCocoShellCompletionCodeProvider>();
        var serviceProvider = services.BuildServiceProvider();

        var generator = serviceProvider.GetRequiredService<IWildCocoShellCompletionCodeProvider>();
        generator.SupportedTargets.Should().BeEquivalentTo("test", "test2");
    }

    [Fact]
    public void CanHandle()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IWildCocoShellCompletionCodeGenerator, TestCodeGenerator>();
        services.AddSingleton<IWildCocoShellCompletionCodeGenerator, Test2CodeGenerator>();
        services.AddSingleton<IWildCocoShellCompletionCodeProvider, WildCocoShellCompletionCodeProvider>();
        var serviceProvider = services.BuildServiceProvider();

        var generator = serviceProvider.GetRequiredService<IWildCocoShellCompletionCodeProvider>();
        generator.CanHandle("test").Should().BeTrue();
        generator.CanHandle("test2").Should().BeTrue();
        generator.CanHandle("test3").Should().BeFalse();
    }

    [Fact]
    public void Generate()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IWildCocoShellCompletionCodeGenerator, TestCodeGenerator>();
        services.AddSingleton<IWildCocoShellCompletionCodeGenerator, Test2CodeGenerator>();
        services.AddSingleton<IWildCocoShellCompletionCodeProvider, WildCocoShellCompletionCodeProvider>();
        var serviceProvider = services.BuildServiceProvider();

        var generator = serviceProvider.GetRequiredService<IWildCocoShellCompletionCodeProvider>();
        var writer = new StringWriter();
        generator.Generate("test2", writer, new CommandCollection(Array.Empty<CommandDescriptor>()));
        writer.ToString().Should().Be("Provider2");
    }

    public class TestCodeGenerator : IWildCocoShellCompletionCodeGenerator
    {
        public IReadOnlyList<string> Targets => new[] {"test"};

        public void Generate(TextWriter writer, CommandCollection commandCollection)
        {
            writer.Write("Provider");
        }

        public void GenerateOnTheFlyCandidates(TextWriter writer, IReadOnlyList<CompletionCandidateValue> values)
        {
            writer.Write("Provider/OnTheFly");
        }
    }

    public class Test2CodeGenerator : IWildCocoShellCompletionCodeGenerator
    {
        public IReadOnlyList<string> Targets => new[] { "test2" };

        public void Generate(TextWriter writer, CommandCollection commandCollection)
        {
            writer.Write("Provider2");
        }

        public void GenerateOnTheFlyCandidates(TextWriter writer, IReadOnlyList<CompletionCandidateValue> values)
        {
            writer.Write("Provider2/OnTheFly");
        }
    }
}
