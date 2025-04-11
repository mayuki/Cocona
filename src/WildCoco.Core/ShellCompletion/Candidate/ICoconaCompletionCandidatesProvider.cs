using WildCoco.CommandLine;

namespace WildCoco.ShellCompletion.Candidate;

/// <summary>
/// Provides shell completion candidates at a shell script generation.
/// </summary>
public interface IWildCocoCompletionStaticCandidatesProvider
{
    CompletionCandidateResult GetCandidates(WildCocoCompletionCandidatesMetadata metadata);
}

/// <summary>
/// Provides shell completion candidates while a user interacted in a shell.
/// </summary>
public interface IWildCocoCompletionOnTheFlyCandidatesProvider
{
    IReadOnlyList<CompletionCandidateValue> GetCandidates(WildCocoCompletionCandidatesMetadata metadata, ParsedCommandLine parsedCommandLine);
}
