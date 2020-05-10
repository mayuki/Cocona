using System;
using System.Collections.Generic;
using System.Linq;

namespace Cocona.ShellCompletion.Candidate
{
    public class CompletionCandidateResult
    {
        public static CompletionCandidateResult Default { get; } = new CompletionCandidateResult(CompletionCandidateResultType.Default);
        public static CompletionCandidateResult File { get; } = new CompletionCandidateResult(CompletionCandidateResultType.File);
        public static CompletionCandidateResult Directory { get; } = new CompletionCandidateResult(CompletionCandidateResultType.Directory);

        public CompletionCandidateResultType ResultType { get; }
        public IReadOnlyList<CompletionCandidateValue> Values { get; }

        public CompletionCandidateResult(CompletionCandidateResultType resultType)
        {
            ResultType = resultType;
            Values = Array.Empty<CompletionCandidateValue>();
        }

        private CompletionCandidateResult(CompletionCandidateResultType resultType, IEnumerable<CompletionCandidateValue> values)
        {
            ResultType = resultType;
            Values = values.ToArray();
        }

        public static CompletionCandidateResult Keywords(IEnumerable<CompletionCandidateValue> values)
            => new CompletionCandidateResult(CompletionCandidateResultType.Keywords, values);

        public static CompletionCandidateResult Keywords(params CompletionCandidateValue[] values)
            => new CompletionCandidateResult(CompletionCandidateResultType.Keywords, values);
    }
}
