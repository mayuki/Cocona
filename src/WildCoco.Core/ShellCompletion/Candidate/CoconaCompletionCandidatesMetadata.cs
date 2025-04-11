using WildCoco.Command;

namespace WildCoco.ShellCompletion.Candidate;

/// <summary>
/// Provides metadata of shell completion state to generate candidates.
/// </summary>
public class WildCocoCompletionCandidatesMetadata
{
    public CompletionCandidateType CandidateType { get; }
    public Type ParameterType { get; }
    public Type CandidatesProviderType { get; }
    public IReadOnlyList<Attribute> ParameterAttributes { get; }
    public CommandOptionDescriptor? CommandOption { get; }
    public CommandArgumentDescriptor? CommandArgument { get; }

    public WildCocoCompletionCandidatesMetadata(
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

    public WildCocoCompletionCandidatesMetadata(
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
