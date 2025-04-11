using WildCoco.Command;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Candidate.Providers;
using WildCoco.Command;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Candidate.Providers;

namespace WildCoco.Test.ShellCompletion.Candidate;

public class StaticKeywordsCompletionCandidatesProviderTest
{
    [Fact]
    public void NoAttribute()
    {
        var argument = new CommandArgumentDescriptor(
            typeof(int),
            "test",
            0,
            "",
            WildCocoDefaultValue.None,
            new Attribute[]
            {
            }
        );
        var metadata = new WildCocoCompletionCandidatesMetadata(
            CompletionCandidateType.Provider,
            typeof(StaticKeywordsCompletionCandidatesProvider),
            argument
        );

        var provider = new StaticKeywordsCompletionCandidatesProvider();
        var candidates = provider.GetCandidates(metadata);
        candidates.ResultType.Should().Be(CompletionCandidateResultType.Default);
        candidates.Values.Should().BeEmpty();
    }

    [Fact]
    public void HasAttribute()
    {
        var argument = new CommandArgumentDescriptor(
            typeof(int),
            "test",
            0,
            "",
            WildCocoDefaultValue.None,
            new Attribute[]
            {
                new CompletionCandidatesAttribute(new [] { "Alice", "Karen" }),
            }
        );
        var metadata = new WildCocoCompletionCandidatesMetadata(
            CompletionCandidateType.Provider,
            typeof(StaticKeywordsCompletionCandidatesProvider),
            argument
        );

        var provider = new StaticKeywordsCompletionCandidatesProvider();
        var candidates = provider.GetCandidates(metadata);
        candidates.ResultType.Should().Be(CompletionCandidateResultType.Keywords);
        candidates.Values.Should().HaveCount(2);
        candidates.Values[0].Value.Should().Be("Alice");
        candidates.Values[1].Value.Should().Be("Karen");
    }
}
