using System;
using System.Collections.Generic;
using System.Text;
using Cocona.Command;

namespace Cocona.ShellCompletion.Candidate
{
    public interface ICoconaCompletionCandidates
    {
        StaticCompletionCandidates GetStaticCandidatesFromOption(CommandOptionDescriptor option);
        StaticCompletionCandidates GetStaticCandidatesFromArgument(CommandArgumentDescriptor argument);
    }

    public class CoconaCompletionCandidates : ICoconaCompletionCandidates
    {
        private readonly ICoconaCompletionCandidatesMetadataFactory _completionCandidatesMetadataFactory;
        private readonly ICoconaCompletionCandidatesProviderFactory _completionCandidatesProviderFactory;

        public CoconaCompletionCandidates(
            ICoconaCompletionCandidatesMetadataFactory completionCandidatesMetadataFactory,
            ICoconaCompletionCandidatesProviderFactory completionCandidatesProviderFactory
        )
        {
            _completionCandidatesMetadataFactory = completionCandidatesMetadataFactory;
            _completionCandidatesProviderFactory = completionCandidatesProviderFactory;
        }

        public StaticCompletionCandidates GetStaticCandidatesFromOption(CommandOptionDescriptor option)
        {
            return GetStaticCandidatesCore(_completionCandidatesMetadataFactory.GetMetadata(option));
        }

        public StaticCompletionCandidates GetStaticCandidatesFromArgument(CommandArgumentDescriptor argument)
        {
            return GetStaticCandidatesCore(_completionCandidatesMetadataFactory.GetMetadata(argument));
        }

        private StaticCompletionCandidates GetStaticCandidatesCore(CoconaCompletionCandidatesMetadata metadata)
        {
            if (metadata.CandidateType == CompletionCandidateType.Provider &&
                typeof(ICoconaCompletionOnTheFlyCandidatesProvider).IsAssignableFrom(metadata.CandidatesProviderType))
            {
                return new StaticCompletionCandidates(metadata.CandidatesProviderType!);
            }
            else
            {
                var candidatesProvider = _completionCandidatesProviderFactory.CreateStaticProvider(metadata);
                var result = candidatesProvider.GetCandidates(metadata);
                return new StaticCompletionCandidates(result);
            }
        }
    }

    public class StaticCompletionCandidates
    {
        public bool IsOnTheFly { get; }
        public CompletionCandidateResult? Result { get; }
        public Type? CandidatesProviderType { get; }

        public StaticCompletionCandidates(CompletionCandidateResult result)
        {
            IsOnTheFly = false;
            Result = result;
            CandidatesProviderType = null;
        }

        public StaticCompletionCandidates(Type candidatesProviderType)
        {
            IsOnTheFly = true;
            Result = null;
            CandidatesProviderType = candidatesProviderType;
        }
    }
}
