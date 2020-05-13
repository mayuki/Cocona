using System.Collections.Generic;
using System.Text;
using Cocona.Command;

namespace Cocona.ShellCompletion.Candidate
{
    public interface ICoconaCompletionCandidates
    {
        StaticCompletionCandidates GetStaticCandidatesFromOption(CommandOptionDescriptor option);
        StaticCompletionCandidates GetStaticCandidatesFromArgument(CommandArgumentDescriptor argument);
        IReadOnlyList<CompletionCandidateValue> GetOnTheFlyCandidates(string paramName, int argSkipCount, int curPos, string? candidateHint);
    }
}
