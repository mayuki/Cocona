namespace Cocona.ShellCompletion.Candidate
{
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
