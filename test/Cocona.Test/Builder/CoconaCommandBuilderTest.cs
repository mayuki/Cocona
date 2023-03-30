using Cocona.Builder;
using Cocona.Builder.Metadata;
using Cocona.Filters;

namespace Cocona.Test.Builder;

public class CoconaCommandBuilderTest
{
    [Fact]
    public void AddCommand_Single()
    {
        var builder = new CoconaCommandsBuilder();
        builder.AddCommand(() => { });

        var built = builder.Build();
        built.Should().HaveCount(1);
        built[0].Should().BeOfType<DelegateCommandData>();
        built[0].Metadata.Should().HaveCount(2);
        built[0].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        built[0].Metadata[1].Should().BeOfType<PrimaryCommandAttribute>();
    }

    [Fact]
    public void AddCommand_Many()
    {
        var builder = new CoconaCommandsBuilder();
        builder.AddCommand("foo", () => { });
        builder.AddCommand("bar", () => { });
        builder.AddCommand("baz", () => { });

        var built = builder.Build();
        built.Should().HaveCount(3);
        built[0].Should().BeOfType<DelegateCommandData>();
        built[1].Should().BeOfType<DelegateCommandData>();
        built[2].Should().BeOfType<DelegateCommandData>();
    }

    [Fact]
    public void NamedAddCommand()
    {
        var builder = new CoconaCommandsBuilder();
        builder.AddCommand("command", () => { });

        var built = builder.Build();
        built.Should().HaveCount(1);
        built[0].Should().BeOfType<DelegateCommandData>();
        ((DelegateCommandData)built[0]).Metadata.Should().HaveCount(2);
        ((DelegateCommandData)built[0]).Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((DelegateCommandData)built[0]).Metadata[1].Should().BeOfType<CommandNameMetadata>().Subject.Name.Should().Be("command");
    }

    [Fact]
    public void SubCommand()
    {
        var builder = new CoconaCommandsBuilder();
        builder.AddSubCommand("sub-command", x =>
        {
            x.AddCommand("command1", () => { });
            x.AddCommand("command2", () => { });
        });

        var built = builder.Build();
        built.Should().HaveCount(1);
        built[0].Should().BeOfType<SubCommandData>();
        ((SubCommandData)built[0]).Metadata.Should().HaveCount(2);
        ((SubCommandData)built[0]).Metadata[0].Should().BeOfType< CommandFromBuilderMetadata>();
        ((SubCommandData)built[0]).Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("sub-command");
        ((SubCommandData)built[0]).Children.Should().HaveCount(2);
        ((SubCommandData)built[0]).Children[0].Should().BeOfType<DelegateCommandData>();
        ((SubCommandData)built[0]).Children[0].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((SubCommandData)built[0]).Children[0].Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("command1");
        ((SubCommandData)built[0]).Children[1].Should().BeOfType<DelegateCommandData>();
        ((SubCommandData)built[0]).Children[1].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((SubCommandData)built[0]).Children[1].Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("command2");
    }

    [Fact]
    public void NestedSubCommand()
    {
        var builder = new CoconaCommandsBuilder();
        builder.AddSubCommand("sub-command", x =>
        {
            x.AddCommand("command1", () => { });
            x.AddCommand("command2", () => { });
            x.AddSubCommand("sub-command-nested", x =>
            {
                x.AddCommand("command3", () => { });
                x.AddCommand("command4", () => { });
                x.AddSubCommand("sub-command-nested-nested", x =>
                {
                    x.AddCommand("command5", () => { });
                    x.AddCommand("command6", () => { });
                });
            });
        });

        var built = builder.Build();
        built.Should().HaveCount(1);
        built[0].Should().BeOfType<SubCommandData>();

        // sub-command
        ((SubCommandData)built[0]).Metadata.Should().HaveCount(2);
        ((SubCommandData)built[0]).Metadata[0].Should().BeOfType< CommandFromBuilderMetadata>();
        ((SubCommandData)built[0]).Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("sub-command");
        ((SubCommandData)built[0]).Children.Should().HaveCount(3);
        ((SubCommandData)built[0]).Children[0].Should().BeOfType<DelegateCommandData>();
        ((SubCommandData)built[0]).Children[0].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((SubCommandData)built[0]).Children[0].Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("command1");
        ((SubCommandData)built[0]).Children[1].Should().BeOfType<DelegateCommandData>();
        ((SubCommandData)built[0]).Children[1].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((SubCommandData)built[0]).Children[1].Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("command2");

        // sub-command-nested
        ((SubCommandData)built[0]).Children[2].Metadata.Should().HaveCount(2);
        ((SubCommandData)built[0]).Children[2].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((SubCommandData)built[0]).Children[2].Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("sub-command-nested");
        ((SubCommandData)((SubCommandData)built[0]).Children[2]).Children.Should().HaveCount(3);
        ((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[0].Should().BeOfType<DelegateCommandData>();
        ((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[0].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[0].Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("command3");
        ((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[1].Should().BeOfType<DelegateCommandData>();
        ((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[1].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[1].Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("command4");

        // sub-command-nested-nested
        ((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[2].Metadata.Should().HaveCount(2);
        ((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[2].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[2].Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("sub-command-nested-nested");
        ((SubCommandData)((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[2]).Children.Should().HaveCount(2);
        ((SubCommandData)((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[2]).Children[0].Should().BeOfType<DelegateCommandData>();
        ((SubCommandData)((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[2]).Children[0].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((SubCommandData)((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[2]).Children[0].Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("command5");
        ((SubCommandData)((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[2]).Children[1].Should().BeOfType<DelegateCommandData>();
        ((SubCommandData)((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[2]).Children[1].Metadata[0].Should().BeOfType<CommandFromBuilderMetadata>();
        ((SubCommandData)((SubCommandData)((SubCommandData)built[0]).Children[2]).Children[2]).Children[1].Metadata[1].Should().BeOfType<CommandNameMetadata>().Which.Name.Should().Be("command6");
    }

    [Fact]
    public void AddCommands()
    {
        var builder = new CoconaCommandsBuilder();
        builder.AddCommands<MyCommands>();

        var built = builder.Build();
        built.Should().HaveCount(1);
        built[0].Should().BeOfType<TypeCommandData>().Subject.Type.Should().Be<MyCommands>();
    }

    class MyCommands
    {
        public void Hello() => Console.WriteLine("Hello");
    }

    [Fact]
    public void Filter_Builder_UseFilter()
    {
        var builder = new CoconaCommandsBuilder();
        builder.AddCommand("Command1", () => { });
        builder.UseFilter(new UseFilter_Filter1());
        builder.AddCommand("Command2", () => { });
        builder.UseFilter(new UseFilter_Filter2());
        builder.AddCommands<MyCommands>();
        builder.UseFilter(new UseFilter_Filter3());

        var built = builder.Build();
        built[0].Metadata.Where(x => x is IFilterMetadata || x is IFilterFactory).Should().HaveCount(0);
        built[1].Metadata.Where(x => x is IFilterMetadata || x is IFilterFactory).Should().HaveCount(1);
        built[2].Metadata.Where(x => x is IFilterMetadata || x is IFilterFactory).Should().HaveCount(2);
    }
    class UseFilter_Filter1 : IFilterMetadata
    {}
    class UseFilter_Filter2 : IFilterFactory
    {
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) => throw new NotImplementedException();
    }
    class UseFilter_Filter3 : IFilterMetadata
    { }
}