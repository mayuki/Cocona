namespace WildCoco.ShellCompletion.Candidate.Providers;

public sealed class StaticCompletionCandidatesProvider : IWildCocoCompletionStaticCandidatesProvider
{
    public CompletionCandidateResult GetCandidates(WildCocoCompletionCandidatesMetadata metadata)
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
