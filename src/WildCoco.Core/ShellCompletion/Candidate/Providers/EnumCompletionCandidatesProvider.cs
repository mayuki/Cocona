namespace WildCoco.ShellCompletion.Candidate.Providers;

public sealed class EnumCompletionCandidatesProvider : IWildCocoCompletionStaticCandidatesProvider
{
    public CompletionCandidateResult GetCandidates(WildCocoCompletionCandidatesMetadata metadata)
        => GetCandidates(metadata.ParameterType);

    private CompletionCandidateResult GetCandidates(Type type)
    {
        if (!type.IsEnum) return CompletionCandidateResult.Default;
        return CompletionCandidateResult.Keywords(Enum.GetNames(type).Select(x => new CompletionCandidateValue(x, string.Empty)));
    }
}
