using Cocona.CommandLine;

namespace Cocona.ShellCompletion.Candidate
{
    /// <summary>
    /// Provides shell completion candidates at a shell script generation.
    /// </summary>
    public interface ICoconaCompletionStaticCandidatesProvider
    {
        CompletionCandidateResult GetCandidates(CoconaCompletionCandidatesMetadata metadata);
    }

    /// <summary>
    /// Provides shell completion candidates while a user interacted in a shell.
    /// </summary>
    public interface ICoconaCompletionOnTheFlyCandidatesProvider
    {
        IReadOnlyList<CompletionCandidateValue> GetCandidates(CoconaCompletionCandidatesMetadata metadata, ParsedCommandLine parsedCommandLine);
    }
}
