using Cocona.Command;
using Cocona.ShellCompletion.Candidate.Providers;

namespace Cocona.ShellCompletion.Candidate
{
    public class CoconaCompletionCandidatesMetadataFactory : ICoconaCompletionCandidatesMetadataFactory
    {
        public CoconaCompletionCandidatesMetadata GetMetadata(CommandOptionDescriptor option)
        {
            var attr = option.ParameterAttributes.OfType<CompletionCandidatesAttribute>().SingleOrDefault();
            if (attr == null)
            {
                if (option.OptionType.IsEnum)
                {
                    return new CoconaCompletionCandidatesMetadata(
                        CompletionCandidateType.Provider,
                        typeof(EnumCompletionCandidatesProvider),
                        option
                    );
                }

                return new CoconaCompletionCandidatesMetadata(
                    CompletionCandidateType.Default,
                    typeof(StaticCompletionCandidatesProvider),
                    option
                );
            }
            else
            {
                return new CoconaCompletionCandidatesMetadata(
                    attr.CandidateType,
                    attr.CandidatesProviderType,
                    option
                );
            }
        }

        public CoconaCompletionCandidatesMetadata GetMetadata(CommandArgumentDescriptor argument)
        {
            var attr = argument.ParameterAttributes.OfType<CompletionCandidatesAttribute>().SingleOrDefault();
            if (attr == null)
            {
                if (argument.ArgumentType.IsEnum)
                {
                    return new CoconaCompletionCandidatesMetadata(
                        CompletionCandidateType.Provider,
                        typeof(EnumCompletionCandidatesProvider),
                        argument
                    );
                }

                return new CoconaCompletionCandidatesMetadata(
                    CompletionCandidateType.Default,
                    typeof(StaticCompletionCandidatesProvider),
                    argument
                );
            }
            else
            {
                return new CoconaCompletionCandidatesMetadata(
                    attr.CandidateType,
                    attr.CandidatesProviderType,
                    argument
                );
            }
        }
    }
}
