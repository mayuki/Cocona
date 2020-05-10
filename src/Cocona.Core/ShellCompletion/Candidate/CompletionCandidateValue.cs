using System;

namespace Cocona.ShellCompletion.Candidate
{
    public class CompletionCandidateValue : IEquatable<CompletionCandidateValue>
    {
        public string Value { get; }
        public string Description { get; }

        public CompletionCandidateValue(string value, string description)
        {
            Value = value;
            Description = description;
        }

        public bool Equals(CompletionCandidateValue other)
        {
            return Value == other.Value && Description == other.Description;
        }

        public override bool Equals(object obj)
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
