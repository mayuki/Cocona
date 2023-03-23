using System;
using System.Collections.Generic;
using System.Linq;
using Cocona.Command;

namespace Cocona.ShellCompletion.Candidate
{
    public class CoconaCompletionCandidates : ICoconaCompletionCandidates
    {
        private readonly ICoconaCompletionCandidatesMetadataFactory _completionCandidatesMetadataFactory;
        private readonly ICoconaCompletionCandidatesProviderFactory _completionCandidatesProviderFactory;
        private readonly ICoconaCommandResolver _commandResolver;
        private readonly ICoconaCommandProvider _commandProvider;

        public CoconaCompletionCandidates(
            ICoconaCompletionCandidatesMetadataFactory completionCandidatesMetadataFactory,
            ICoconaCompletionCandidatesProviderFactory completionCandidatesProviderFactory,
            ICoconaCommandResolver commandResolver,
            ICoconaCommandProvider commandProvider
        )
        {
            _completionCandidatesMetadataFactory = completionCandidatesMetadataFactory;
            _completionCandidatesProviderFactory = completionCandidatesProviderFactory;
            _commandResolver = commandResolver;
            _commandProvider = commandProvider;
        }

        public IReadOnlyList<CompletionCandidateValue> GetOnTheFlyCandidates(string paramName, IReadOnlyList<string> args, int curPos, string? candidateHint)
        {
            var result = _commandResolver.ParseAndResolve(_commandProvider.GetCommandCollection(), args);

            if (result.Success)
            {
                var metadata = default(CoconaCompletionCandidatesMetadata);
                var option = result.MatchedCommand!.Options.FirstOrDefault(x => x.Name == paramName);
                if (option != null)
                {
                    metadata = _completionCandidatesMetadataFactory.GetMetadata(option);
                }
                else
                {
                    var argument = result.MatchedCommand!.Arguments.FirstOrDefault(x => x.Name == paramName);
                    if (argument != null)
                    {
                        metadata = _completionCandidatesMetadataFactory.GetMetadata(argument);
                    }
                }

                if (metadata != null)
                {
                    return _completionCandidatesProviderFactory.CreateOnTheFlyProvider(metadata).GetCandidates(metadata, result.ParsedCommandLine!);
                }
            }

            return Array.Empty<CompletionCandidateValue>();
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
}
