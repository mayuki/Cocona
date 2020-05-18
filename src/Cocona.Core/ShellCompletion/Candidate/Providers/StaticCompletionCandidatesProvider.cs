using System;

namespace Cocona.ShellCompletion.Candidate.Providers
{
    public sealed class StaticCompletionCandidatesProvider : ICoconaCompletionStaticCandidatesProvider
    {
        public CompletionCandidateResult GetCandidates(CoconaCompletionCandidatesMetadata metadata)
        {
            return metadata.CandidateType switch
            {
                CompletionCandidateType.Default => CompletionCandidateResult.Default,
                CompletionCandidateType.Directory => CompletionCandidateResult.Directory,
                CompletionCandidateType.File => CompletionCandidateResult.File,
                _ => throw new NotSupportedException(),
            };
        }
    }
}
