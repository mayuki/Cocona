using WildCoco.Command;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Candidate.Providers;
using WildCoco.Command;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Candidate.Providers;

namespace WildCoco.Test.ShellCompletion.Candidate;

public class EnumCompletionCandidatesProviderTest
{
    [Fact]
    public void Option_Default()
    {
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
        var metadata = new WildCocoCompletionCandidatesMetadata(
            CompletionCandidateType.Provider,
            typeof(StaticCompletionCandidatesProvider),
            option
        );
        var provider = new EnumCompletionCandidatesProvider();
        var candidates = provider.GetCandidates(metadata);
        candidates.ResultType.Should().Be(CompletionCandidateResultType.Default);
    }

    [Fact]
    public void Option_EnumValues()
    {
        var option = new CommandOptionDescriptor(
            typeof(TestEnum),
            "test",
            Array.Empty<char>(),
            "",
            WildCocoDefaultValue.None,
            string.Empty,
            CommandOptionFlags.None,
            new Attribute[] { }
        );
        var metadata = new WildCocoCompletionCandidatesMetadata(
            CompletionCandidateType.Provider,
            typeof(EnumCompletionCandidatesProvider),
            option
        );

        var provider = new EnumCompletionCandidatesProvider();
        var candidates = provider.GetCandidates(metadata);
        candidates.ResultType.Should().Be(CompletionCandidateResultType.Keywords);
        candidates.Values.Should().HaveCount(2);
        candidates.Values[0].Value.Should().Be("Alice");
        candidates.Values[0].Description.Should().Be(string.Empty);
        candidates.Values[1].Value.Should().Be("Karen");
        candidates.Values[1].Description.Should().Be(string.Empty);
    }

    [Fact]
    public void Argument_Default()
    {
        var argument = new CommandArgumentDescriptor(
            typeof(int),
            "test",
            0,
            "",
            WildCocoDefaultValue.None,
            new Attribute[] { }
        );
        var metadata = new WildCocoCompletionCandidatesMetadata(
            CompletionCandidateType.Provider,
            typeof(EnumCompletionCandidatesProvider),
            argument
        );

        var provider = new EnumCompletionCandidatesProvider();
        var candidates = provider.GetCandidates(metadata);
        candidates.ResultType.Should().Be(CompletionCandidateResultType.Default);
    }

    [Fact]
    public void Argument_EnumValues()
    {
        var argument = new CommandArgumentDescriptor(
            typeof(TestEnum),
            "test",
            0,
            "",
            WildCocoDefaultValue.None,
            new Attribute[] { }
        );
        var metadata = new WildCocoCompletionCandidatesMetadata(
            CompletionCandidateType.Provider,
            typeof(EnumCompletionCandidatesProvider),
            argument
        );

        var provider = new EnumCompletionCandidatesProvider();
        var candidates = provider.GetCandidates(metadata);
        candidates.ResultType.Should().Be(CompletionCandidateResultType.Keywords);
        candidates.Values.Should().HaveCount(2);
        candidates.Values[0].Value.Should().Be("Alice");
        candidates.Values[0].Description.Should().Be(string.Empty);
        candidates.Values[1].Value.Should().Be("Karen");
        candidates.Values[1].Description.Should().Be(string.Empty);
    }

    enum TestEnum
    {
        Alice,
        Karen
    }
}
