using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cocona.Application;
using Cocona.Command;
using Cocona.ShellCompletion;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cocona.Test.ShellCompletion
{
    public class BashCoconaShellCompletionCodeProviderTest
    {
        [Fact]
        public void Generate()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICoconaApplicationMetadataProvider, CoconaApplicationMetadataProvider>();
            services.AddSingleton<ICoconaShellCompletionCodeProvider, BashCoconaShellCompletionCodeProvider>();
            var serviceProvider = services.BuildServiceProvider();

            var provider = serviceProvider.GetRequiredService<ICoconaShellCompletionCodeProvider>();
            provider.Targets.Should().BeEquivalentTo("bash");

            var writer = new StringWriter();
            provider.Generate(writer, new CommandCollection(Array.Empty<CommandDescriptor>()));
        }
    }
}
