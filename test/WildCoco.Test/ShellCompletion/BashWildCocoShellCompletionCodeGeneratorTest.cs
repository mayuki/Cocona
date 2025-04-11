using WildCoco.Application;
using WildCoco.Command;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Generators;
using WildCoco.Application;
using WildCoco.Command;
using WildCoco.ShellCompletion.Candidate;
using WildCoco.ShellCompletion.Generators;

namespace WildCoco.Test.ShellCompletion;

public class BashWildCocoShellCompletionCodeGeneratorTest
{
    [Fact]
    public void Generate()
    {
        var provider = new BashWildCocoShellCompletionCodeGenerator(new WildCocoApplicationMetadataProvider(), new TestCompletionCandidates());
        provider.Targets.Should().BeEquivalentTo("bash");

        var writer = new StringWriter();
        provider.Generate(writer, new CommandCollection(Array.Empty<CommandDescriptor>()));
    }

    class TestCompletionCandidates : IWildCocoCompletionCandidates
    {
        public StaticCompletionCandidates GetStaticCandidatesFromOption(CommandOptionDescriptor option)
        {
            return new StaticCompletionCandidates(CompletionCandidateResult.Default);
        }

        public StaticCompletionCandidates GetStaticCandidatesFromArgument(CommandArgumentDescriptor argument)
        {
            return new StaticCompletionCandidates(CompletionCandidateResult.Default);
        }

        public IReadOnlyList<CompletionCandidateValue> GetOnTheFlyCandidates(string paramName, IReadOnlyList<string> arguments, int curPos, string? candidateHint)
        {
            throw new NotImplementedException();
        }
    }
}
