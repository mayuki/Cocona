using System;
using System.Collections.Generic;
using System.Linq;

namespace Cocona.ShellCompletion.Candidate.Providers
{
    public sealed class EnumCompletionCandidatesProvider : ICoconaCompletionStaticCandidatesProvider
    {
        public CompletionCandidateResult GetCandidates(CoconaCompletionCandidatesMetadata metadata)
            => GetCandidates(metadata.ParameterType);

        private CompletionCandidateResult GetCandidates(Type type)
        {
            if (!type.IsEnum) return CompletionCandidateResult.Default;
            return CompletionCandidateResult.Keywords(Enum.GetNames(type).Select(x => new CompletionCandidateValue(x, string.Empty)));
        }
    }
}
