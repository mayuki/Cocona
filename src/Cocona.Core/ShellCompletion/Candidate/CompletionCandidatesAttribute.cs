using Cocona.ShellCompletion.Candidate.Providers;

namespace Cocona.ShellCompletion.Candidate;

/// <summary>
/// Specifies the parameter that provides shell completion candidates.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class CompletionCandidatesAttribute : Attribute, ICoconaCompletionCandidatesStaticKeywords
{
    private readonly CompletionCandidateValue[] _candidates;

    /// <summary>
    /// Gets the candidate type of the parameter.
    /// </summary>
    public CompletionCandidateType CandidateType { get; }

    /// <summary>
    /// Gets the type of a candidates provider.
    /// </summary>
    public Type CandidatesProviderType { get; }

    IEnumerable<CompletionCandidateValue> ICoconaCompletionCandidatesStaticKeywords.Candidates => _candidates;

    /// <summary>
    /// Initialize a new instance of the <see cref="CompletionCandidatesAttribute"/> class with a specified <see cref="CompletionCandidateType"/>.
    /// </summary>
    /// <param name="candidateType">The candidate type of the parameter.</param>
    public CompletionCandidatesAttribute(CompletionCandidateType candidateType)
    {
        switch (candidateType)
        {
            case CompletionCandidateType.Default:
            case CompletionCandidateType.File:
            case CompletionCandidateType.Directory:
                break;
            default:
                throw new InvalidOperationException($"CompletionCandidateType '{candidateType}' requires a custom provider type.");
        }

        _candidates = Array.Empty<CompletionCandidateValue>();
        CandidateType = candidateType;
        CandidatesProviderType = typeof(StaticCompletionCandidatesProvider);
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="CompletionCandidatesAttribute"/> class with a specified <see cref="Type"/> of the candidates provider.
    /// </summary>
    /// <param name="typeOfProvider">The <see cref="Type"/> of the candidates provider.</param>
    public CompletionCandidatesAttribute(Type typeOfProvider)
    {
        if (!typeof(ICoconaCompletionOnTheFlyCandidatesProvider).IsAssignableFrom(typeOfProvider) &&
            !typeof(ICoconaCompletionStaticCandidatesProvider).IsAssignableFrom(typeOfProvider))
        {
            throw new ArgumentException($"The type '{typeOfProvider.FullName}' must implement ICoconaCompletionStaticCandidatesProvider or ICoconaCompletionOnTheFlyCandidatesProvider.");
        }

        CandidateType = CompletionCandidateType.Provider;
        _candidates = Array.Empty<CompletionCandidateValue>();
        CandidatesProviderType = typeOfProvider;
    }


    /// <summary>
    /// Initialize a new instance of the <see cref="CompletionCandidatesAttribute"/> class with a specified candidates.
    /// </summary>
    /// <param name="candidates">The candidate values</param>
    public CompletionCandidatesAttribute(string[] candidates)
    {
        CandidateType = CompletionCandidateType.Provider;
        CandidatesProviderType = typeof(StaticKeywordsCompletionCandidatesProvider);
        _candidates = candidates?.Select(x => new CompletionCandidateValue(x, string.Empty)).ToArray() ?? throw new ArgumentNullException(nameof(candidates));
    }
}