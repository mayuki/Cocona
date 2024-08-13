namespace Cocona.ShellCompletion.Candidate.Providers;

public sealed class EnumCompletionCandidatesProvider : ICoconaCompletionStaticCandidatesProvider
{
    public CompletionCandidateResult GetCandidates(CoconaCompletionCandidatesMetadata metadata)
    {
        var nullableEnum = Nullable.GetUnderlyingType(metadata.ParameterType);
        return GetCandidates(nullableEnum ?? metadata.ParameterType);
    }

    private CompletionCandidateResult GetCandidates(Type type)
    {
        if (!type.IsEnum) return CompletionCandidateResult.Default;
        return CompletionCandidateResult.Keywords(Enum.GetNames(type).Select(x => new CompletionCandidateValue(x, string.Empty)));
    }
}
