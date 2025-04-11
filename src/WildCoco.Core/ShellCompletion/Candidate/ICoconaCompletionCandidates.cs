using WildCoco.Command;

namespace WildCoco.ShellCompletion.Candidate;

public interface IWildCocoCompletionCandidates
{
    StaticCompletionCandidates GetStaticCandidatesFromOption(CommandOptionDescriptor option);
    StaticCompletionCandidates GetStaticCandidatesFromArgument(CommandArgumentDescriptor argument);
    IReadOnlyList<CompletionCandidateValue> GetOnTheFlyCandidates(string paramName, IReadOnlyList<string> args, int curPos, string? candidateHint);
}
