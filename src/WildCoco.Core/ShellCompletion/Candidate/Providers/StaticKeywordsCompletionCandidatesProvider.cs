namespace WildCoco.ShellCompletion.Candidate.Providers;

public sealed class StaticKeywordsCompletionCandidatesProvider : IWildCocoCompletionStaticCandidatesProvider
{
    public CompletionCandidateResult GetCandidates(WildCocoCompletionCandidatesMetadata metadata)
    {
        var attr = metadata.ParameterAttributes.OfType<CompletionCandidatesAttribute>().FirstOrDefault();
        if (attr != null)
        {
            return CompletionCandidateResult.Keywords(((IWildCocoCompletionCandidatesStaticKeywords)attr).Candidates);
        }

        return CompletionCandidateResult.Default;
    }
}

internal interface IWildCocoCompletionCandidatesStaticKeywords
{
    IEnumerable<CompletionCandidateValue> Candidates { get; }
}
