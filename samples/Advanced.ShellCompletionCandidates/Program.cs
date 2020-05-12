using System;
using System.Collections.Generic;
using System.Linq;
using Cocona;
using Cocona.Command;
using Cocona.CommandLine;
using Cocona.ShellCompletion.Candidate;

namespace CoconaSample.Advanced.ShellCompletionCandidates
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public void Inline([CompletionCandidates(new [] { "Foo", "Bar", "Baz" })] string name)
        {
            Console.WriteLine($"Hello {name}!");
        }

        public void Static([CompletionCandidates(typeof(StaticMemberNameProvider))] string name)
        {
            Console.WriteLine($"Hello {name}!");
        }

        public void OnTheFly([CompletionCandidates(typeof(OnTheFlyMemberNameProvider))] string name, string group)
        {
            Console.WriteLine($"Hello {name}!");
        }
    }

    class StaticMemberNameProvider : ICoconaCompletionStaticCandidatesProvider
    {
        private readonly CompletionCandidateResult _results = CompletionCandidateResult.Keywords(
            new CompletionCandidateValue("Karen", ""),
            new CompletionCandidateValue("Alice", ""),
            new CompletionCandidateValue("Shino", ""),
            new CompletionCandidateValue("Yoko", ""),
            new CompletionCandidateValue("Aya", "")
        );

        public CompletionCandidateResult GetCandidates(CoconaCompletionCandidatesMetadata metadata) => _results;
    }

    class OnTheFlyMemberNameProvider : ICoconaCompletionOnTheFlyCandidatesProvider
    {
        private readonly CompletionCandidateValue[] _resultsSphere =
        {
            new CompletionCandidateValue("Aki", ""),
            new CompletionCandidateValue("Minako", ""),
            new CompletionCandidateValue("Haruka", ""),
            new CompletionCandidateValue("Ayahi", "")
        };
        private readonly CompletionCandidateValue[] _resultsTrySail =
        {
            new CompletionCandidateValue("Sora", ""),
            new CompletionCandidateValue("Momo", ""),
            new CompletionCandidateValue("Shiina", "")
        };

        public IReadOnlyList<CompletionCandidateValue> GetCandidates(CoconaCompletionCandidatesMetadata metadata, ParsedCommandLine parsedCommandLine)
            => parsedCommandLine.Options.Any(x => x.Option.Name == "group" && string.Equals(x.Value, "trysail", StringComparison.OrdinalIgnoreCase))
                ? _resultsTrySail
                : _resultsSphere;
    }
}
