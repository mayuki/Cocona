namespace Cocona.ShellCompletion.Candidate
{
    /// <summary>
    /// Result of a shell completion candidate while generating a shell script. It contains candidate values.
    /// </summary>
    public class CompletionCandidateResult
    {
        public static CompletionCandidateResult Default { get; } = new CompletionCandidateResult(CompletionCandidateResultType.Default);
        public static CompletionCandidateResult File { get; } = new CompletionCandidateResult(CompletionCandidateResultType.File);
        public static CompletionCandidateResult Directory { get; } = new CompletionCandidateResult(CompletionCandidateResultType.Directory);

        /// <summary>
        /// Gets the type of this result.
        /// </summary>
        public CompletionCandidateResultType ResultType { get; }

        /// <summary>
        /// Gets the candidates values.
        /// </summary>
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
