using Cocona.Command;

namespace Cocona.ShellCompletion.Candidate;

public interface ICoconaCompletionCandidates
{
    StaticCompletionCandidates GetStaticCandidatesFromOption(CommandOptionDescriptor option);
    StaticCompletionCandidates GetStaticCandidatesFromArgument(CommandArgumentDescriptor argument);
    IReadOnlyList<CompletionCandidateValue> GetOnTheFlyCandidates(string paramName, IReadOnlyList<string> args, int curPos, string? candidateHint);
}