using Cocona.Command;

namespace Cocona.ShellCompletion.Candidate;

/// <summary>
/// Provides metadata of shell completion state to generate candidates.
/// </summary>
public class CoconaCompletionCandidatesMetadata
{
    public CompletionCandidateType CandidateType { get; }
    public Type ParameterType { get; }
    public Type CandidatesProviderType { get; }
    public IReadOnlyList<Attribute> ParameterAttributes { get; }
    public CommandOptionDescriptor? CommandOption { get; }
    public CommandArgumentDescriptor? CommandArgument { get; }

    public CoconaCompletionCandidatesMetadata(
        CompletionCandidateType candidateType,
        Type candidatesProviderType,
        CommandOptionDescriptor commandOption
    )
    {
        CandidateType = candidateType;
        CandidatesProviderType = candidatesProviderType;
        ParameterType = commandOption.OptionType;
        ParameterAttributes = commandOption.ParameterAttributes;
        CommandOption = commandOption;
        CommandArgument = null;
    }

    public CoconaCompletionCandidatesMetadata(
        CompletionCandidateType candidateType,
        Type candidatesProviderType,
        CommandArgumentDescriptor commandArgument
    )
    {
        CandidateType = candidateType;
        CandidatesProviderType = candidatesProviderType;
        ParameterType = commandArgument.ArgumentType;
        ParameterAttributes = commandArgument.ParameterAttributes;
        CommandOption = null;
        CommandArgument = commandArgument;
    }
}