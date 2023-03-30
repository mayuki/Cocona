using Cocona.Command;

namespace Cocona.ShellCompletion.Candidate;

public interface ICoconaCompletionCandidatesMetadataFactory
{
    CoconaCompletionCandidatesMetadata GetMetadata(CommandOptionDescriptor option);
    CoconaCompletionCandidatesMetadata GetMetadata(CommandArgumentDescriptor argument);
}