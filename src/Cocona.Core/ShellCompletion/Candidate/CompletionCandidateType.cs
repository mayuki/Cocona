namespace Cocona.ShellCompletion.Candidate;

/// <summary>
/// Specifies a type of the completion candidate.
/// </summary>
public enum CompletionCandidateType
{
    /// <summary>
    /// The default behavior by the shell itself.
    /// </summary>
    Default,

    /// <summary>
    /// The parameter requires a file path.
    /// </summary>
    File,

    /// <summary>
    /// The parameter requires a directory path.
    /// </summary>
    Directory,

    /// <summary>
    /// The parameter requires a custom value which provided by <see cref="ICoconaCompletionStaticCandidatesProvider"/> or <see cref="ICoconaCompletionOnTheFlyCandidatesProvider"/>.
    /// </summary>
    Provider,
}