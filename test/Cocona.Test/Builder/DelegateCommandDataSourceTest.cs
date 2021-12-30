using System;
using System.Collections.Generic;
using Cocona.Builder;
using Cocona.Builder.Metadata;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.Builder
{
    public class DelegateCommandDataSourceTest
    {
        [Fact]
        public void Build()
        {
            var conventions = new List<Action<ICommandBuilder>>();
            var dataSource = new DelegateCommandDataSource(TestMethod, conventions);
            var data = dataSource.Build();

            var delegateCommandData = data.Should().BeOfType<DelegateCommandData>().Subject;
            delegateCommandData.Target.Should().Be(this);
            delegateCommandData.Method.Should().BeSameAs(new Action(TestMethod).Method);
        }

        [Fact]
        public void AttributesToMetadata()
        {
            var conventions = new List<Action<ICommandBuilder>>();
            var dataSource = new DelegateCommandDataSource(TestMethod, conventions);
            conventions.Add(x => x.Metadata.Add(new CommandNameMetadata("TestMethod")));
            var data = dataSource.Build();
            data.Metadata.Should().HaveCount(3);

            // Attributes -> Conventions
            data.Metadata[0].Should().BeOfType<DummyAttribute>();
            data.Metadata[1].Should().BeOfType<Dummy2Attribute>();
            data.Metadata[2].Should().BeOfType<CommandNameMetadata>();
        }

        [Dummy]
        [Dummy2]
        void TestMethod() => Console.WriteLine("Hello");

        class DummyAttribute : Attribute { }
        class Dummy2Attribute : Attribute { }
    }
}
