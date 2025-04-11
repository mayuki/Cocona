using WildCoco.Command;
using WildCoco.ShellCompletion.Candidate.Providers;

namespace WildCoco.ShellCompletion.Candidate;

public class WildCocoCompletionCandidatesMetadataFactory : IWildCocoCompletionCandidatesMetadataFactory
{
    public WildCocoCompletionCandidatesMetadata GetMetadata(CommandOptionDescriptor option)
    {
        var attr = option.ParameterAttributes.OfType<CompletionCandidatesAttribute>().SingleOrDefault();
        if (attr == null)
        {
            if (option.OptionType.IsEnum)
            {
                return new WildCocoCompletionCandidatesMetadata(
                    CompletionCandidateType.Provider,
                    typeof(EnumCompletionCandidatesProvider),
                    option
                );
            }

            return new WildCocoCompletionCandidatesMetadata(
                CompletionCandidateType.Default,
                typeof(StaticCompletionCandidatesProvider),
                option
            );
        }
        else
        {
            return new WildCocoCompletionCandidatesMetadata(
                attr.CandidateType,
                attr.CandidatesProviderType,
                option
            );
        }
    }

    public WildCocoCompletionCandidatesMetadata GetMetadata(CommandArgumentDescriptor argument)
    {
        var attr = argument.ParameterAttributes.OfType<CompletionCandidatesAttribute>().SingleOrDefault();
        if (attr == null)
        {
            if (argument.ArgumentType.IsEnum)
            {
                return new WildCocoCompletionCandidatesMetadata(
                    CompletionCandidateType.Provider,
                    typeof(EnumCompletionCandidatesProvider),
                    argument
                );
            }

            return new WildCocoCompletionCandidatesMetadata(
                CompletionCandidateType.Default,
                typeof(StaticCompletionCandidatesProvider),
                argument
            );
        }
        else
        {
            return new WildCocoCompletionCandidatesMetadata(
                attr.CandidateType,
                attr.CandidatesProviderType,
                argument
            );
        }
    }
}
