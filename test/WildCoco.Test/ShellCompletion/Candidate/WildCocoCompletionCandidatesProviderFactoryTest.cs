#nullable enable
using WildCoco.Command;
using WildCoco.CommandLine;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.Application;
using WildCoco.Command;
using WildCoco.CommandLine;
using WildCoco.ShellCompletion.Candidate;
using Microsoft.Extensions.DependencyInjection;

namespace WildCoco.Test.ShellCompletion.Candidate;

public class WildCocoCompletionCandidatesProviderFactoryTest
{
    [Fact]
    public void Static()
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var activator = new WildCocoInstanceActivator();

        var option = new CommandOptionDescriptor(
            typeof(int),
            "test",
            Array.Empty<char>(),
            "",
            WildCocoDefaultValue.None,
            string.Empty,
            CommandOptionFlags.None,
            new Attribute[] { }
        );
        var factory = new WildCocoCompletionCandidatesProviderFactory(serviceProvider, activator);
        var metadata = new WildCocoCompletionCandidatesMetadata(
            CompletionCandidateType.Provider,
            typeof(TestStaticProvider),
            option
        );
        var provider = factory.CreateStaticProvider(metadata);
        provider.Should().BeOfType<TestStaticProvider>();
    }

    [Fact]
    public void OnTheFly()
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var activator = new WildCocoInstanceActivator();

        var option = new CommandOptionDescriptor(
            typeof(int),
            "test",
            Array.Empty<char>(),
            "",
            WildCocoDefaultValue.None,
            string.Empty,
            CommandOptionFlags.None,
            new Attribute[] { }
        );
        var factory = new WildCocoCompletionCandidatesProviderFactory(serviceProvider, activator);
        var metadata = new WildCocoCompletionCandidatesMetadata(
            CompletionCandidateType.Provider,
            typeof(TestOnTheFlyProvider),
            option
        );
        var provider = factory.CreateOnTheFlyProvider(metadata);
        provider.Should().BeOfType<TestOnTheFlyProvider>();
    }

    private class TestStaticProvider : IWildCocoCompletionStaticCandidatesProvider
    {
        public CompletionCandidateResult GetCandidates(WildCocoCompletionCandidatesMetadata metadata)
        {
            throw new NotImplementedException();
        }
    }

    private class TestOnTheFlyProvider : IWildCocoCompletionOnTheFlyCandidatesProvider
    {
        public IReadOnlyList<CompletionCandidateValue> GetCandidates(WildCocoCompletionCandidatesMetadata metadata, ParsedCommandLine parsedCommandLine)
        {
            throw new NotImplementedException();
        }
    }
}
