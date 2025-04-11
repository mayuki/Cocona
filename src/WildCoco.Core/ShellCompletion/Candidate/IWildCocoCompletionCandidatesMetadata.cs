using WildCoco.Command;

namespace WildCoco.ShellCompletion.Candidate;

public interface IWildCocoCompletionCandidatesMetadataFactory
{
    WildCocoCompletionCandidatesMetadata GetMetadata(CommandOptionDescriptor option);
    WildCocoCompletionCandidatesMetadata GetMetadata(CommandArgumentDescriptor argument);
}
