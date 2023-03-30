namespace Cocona.ShellCompletion.Candidate
{
    /// <summary>
    /// The value of shell completion candidate.
    /// </summary>
    public class CompletionCandidateValue : IEquatable<CompletionCandidateValue>
    {
        /// <summary>
        /// Gets the candidate value. The value will be show on the screen.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the description of the candidate.
        /// </summary>
        public string Description { get; }

        public CompletionCandidateValue(string value, string description)
        {
            Value = value;
            Description = description;
        }

        public bool Equals(CompletionCandidateValue? other)
        {
            return other is not null && Value == other.Value && Description == other.Description;
        }

        public override bool Equals(object? obj)
        {
            return obj is CompletionCandidateValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Value != null ? Value.GetHashCode() : 0) * 397) ^ (Description != null ? Description.GetHashCode() : 0);
            }
        }
    }
}
