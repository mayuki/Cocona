using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cocona.Command;
using Cocona.ShellCompletion;
using Cocona.ShellCompletion.Candidate;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cocona.Test.ShellCompletion
{
    public class CoconaShellCompletionCodeGeneratorTest
    {
        [Fact]
        public void BuildGenerator()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICoconaShellCompletionCodeProvider, TestCodeProvider>();
            services.AddSingleton<ICoconaShellCompletionCodeProvider, Test2CodeProvider>();
            services.AddSingleton<ICoconaShellCompletionCodeGenerator, CoconaShellCompletionCodeGenerator>();
            var serviceProvider = services.BuildServiceProvider();

            var generator = serviceProvider.GetRequiredService<ICoconaShellCompletionCodeGenerator>();
            generator.SupportedTargets.Should().BeEquivalentTo("test", "test2");
        }

        [Fact]
        public void CanHandle()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICoconaShellCompletionCodeProvider, TestCodeProvider>();
            services.AddSingleton<ICoconaShellCompletionCodeProvider, Test2CodeProvider>();
            services.AddSingleton<ICoconaShellCompletionCodeGenerator, CoconaShellCompletionCodeGenerator>();
            var serviceProvider = services.BuildServiceProvider();

            var generator = serviceProvider.GetRequiredService<ICoconaShellCompletionCodeGenerator>();
            generator.CanHandle("test").Should().BeTrue();
            generator.CanHandle("test2").Should().BeTrue();
            generator.CanHandle("test3").Should().BeFalse();
        }

        [Fact]
        public void Generate()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICoconaShellCompletionCodeProvider, TestCodeProvider>();
            services.AddSingleton<ICoconaShellCompletionCodeProvider, Test2CodeProvider>();
            services.AddSingleton<ICoconaShellCompletionCodeGenerator, CoconaShellCompletionCodeGenerator>();
            var serviceProvider = services.BuildServiceProvider();

            var generator = serviceProvider.GetRequiredService<ICoconaShellCompletionCodeGenerator>();
            var writer = new StringWriter();
            generator.Generate("test2", writer, new CommandCollection(Array.Empty<CommandDescriptor>()));
            writer.ToString().Should().Be("Provider2");
        }

        public class TestCodeProvider : ICoconaShellCompletionCodeProvider
        {
            public IReadOnlyList<string> Targets => new[] {"test"};

            public void Generate(TextWriter writer, CommandCollection commandCollection)
            {
                writer.Write("Provider");
            }

            public void GenerateOnTheFlyCandidates(TextWriter writer, IReadOnlyList<CompletionCandidateValue> values)
            {
                writer.Write("Provider/OnTheFly");
            }
        }

        public class Test2CodeProvider : ICoconaShellCompletionCodeProvider
        {
            public IReadOnlyList<string> Targets => new[] { "test2" };

            public void Generate(TextWriter writer, CommandCollection commandCollection)
            {
                writer.Write("Provider2");
            }

            public void GenerateOnTheFlyCandidates(TextWriter writer, IReadOnlyList<CompletionCandidateValue> values)
            {
                writer.Write("Provider2/OnTheFly");
            }
        }
    }
}
