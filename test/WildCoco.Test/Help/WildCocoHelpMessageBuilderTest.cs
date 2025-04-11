using WildCoco;
using WildCoco.Application;
using WildCoco.Command;
using WildCoco.Command.BuiltIn;
using WildCoco.Command.Features;
using WildCoco.Help;
using WildCoco.Help.DocumentModel;
using WildCoco.Application;
using WildCoco.Command;
using WildCoco.Command.BuiltIn;
using WildCoco.Command.Features;
using WildCoco.Help;
using WildCoco.Help.DocumentModel;

namespace WildCoco.Test.Help;

public class WildCocoHelpMessageBuilderTest
{
    [Fact]
    public void Single()
    {
        // $ ./app-single --help
        var commandProvider = new WildCocoCommandProvider(new[] {typeof(TestCommands_Single) });
        var commandCollectionRoot = commandProvider.GetCommandCollection();
        var commandStack = new[] { commandCollectionRoot.All.First() }; // --help (OptionLikeCommand) pushes the command to the stack.

        var targetCommand = BuiltInOptionLikeCommands.Help.Command;
        var commandCollection = commandCollectionRoot;

        var appContext = new WildCocoAppContext(targetCommand, default);
        appContext.Features.Set<IWildCocoCommandFeature>(new WildCocoCommandFeature(commandCollection, targetCommand, commandStack, new object() /* unused */));
        var helpBuilder = new WildCocoHelpMessageBuilder(
            new FakeAppContextAccessor(appContext),
            new FakeCommandHelpProvider(),
            new FakeHelpRenderer(),
            commandProvider
        );

        var help = helpBuilder.BuildAndRenderForCurrentContext();
        help.Should().Be("IndexHelp;Commands=Foo;SubCommandStack="); // If the collection has only one command, a generated help is always IndexHelp.
    }

    [Fact]
    public void Single_Command()
    {
        // $ ./app-single (build a message in the command)
        var commandProvider = new WildCocoCommandProvider(new[] { typeof(TestCommands_Single) });
        var commandCollectionRoot = commandProvider.GetCommandCollection();
        var commandStack = Array.Empty<CommandDescriptor>();

        var targetCommand = commandCollectionRoot.Primary;
        var commandCollection = commandCollectionRoot;

        var appContext = new WildCocoAppContext(targetCommand, default);
        appContext.Features.Set<IWildCocoCommandFeature>(new WildCocoCommandFeature(commandCollection, targetCommand, commandStack, new object() /* unused */));
        var helpBuilder = new WildCocoHelpMessageBuilder(
            new FakeAppContextAccessor(appContext),
            new FakeCommandHelpProvider(),
            new FakeHelpRenderer(),
            commandProvider
        );

        var help = helpBuilder.BuildAndRenderForCurrentCommand();
        help.Should().Be("IndexHelp;Commands=Foo;SubCommandStack="); // If the collection has only one command, a generated help is always IndexHelp.
    }

    [Fact]
    public void Multiple_Context_Index()
    {
        // $ ./app-multiple --help
        var commandProvider = new WildCocoCommandProvider(new[] { typeof(TestCommands_Multiple) });
        var commandCollectionRoot = commandProvider.GetCommandCollection();
        var commandStack = Array.Empty<CommandDescriptor>();

        var targetCommand = BuiltInPrimaryCommand.GetCommand(string.Empty); // `TestCommands_Multiple` has no primary command. Use BuiltInPrimaryCommand.
        var commandCollection = commandCollectionRoot;

        var appContext = new WildCocoAppContext(targetCommand, default);
        appContext.Features.Set<IWildCocoCommandFeature>(new WildCocoCommandFeature(commandCollection, targetCommand, commandStack, new object() /* unused */));
        var helpBuilder = new WildCocoHelpMessageBuilder(
            new FakeAppContextAccessor(appContext),
            new FakeCommandHelpProvider(),
            new FakeHelpRenderer(),
            commandProvider
        );

        var help = helpBuilder.BuildAndRenderForCurrentContext();
        help.Should().Be("IndexHelp;Commands=Foo,Bar;SubCommandStack=");
    }

    [Fact]
    public void Multiple_Context_Command()
    {
        // $ ./app-multiple Foo --help
        var commandProvider = new WildCocoCommandProvider(new[] { typeof(TestCommands_Multiple) });
        var commandCollectionRoot = commandProvider.GetCommandCollection();
        var commandStack = new [] { commandCollectionRoot.All.First(x => x.Name == "Foo") }; // --help (OptionLikeCommand) pushes the command to the stack.

        var targetCommand = BuiltInOptionLikeCommands.Help.Command;
        var commandCollection = commandCollectionRoot;

        var appContext = new WildCocoAppContext(targetCommand, default);
        appContext.Features.Set<IWildCocoCommandFeature>(new WildCocoCommandFeature(commandCollection, targetCommand, commandStack, new object() /* unused */));
        var helpBuilder = new WildCocoHelpMessageBuilder(
            new FakeAppContextAccessor(appContext),
            new FakeCommandHelpProvider(),
            new FakeHelpRenderer(),
            commandProvider
        );

        var help = helpBuilder.BuildAndRenderForCurrentContext();
        help.Should().Be("Command;Name=Foo;SubCommandStack=");
    }

    [Fact]
    public void Multiple_Command()
    {
        // $ ./app-multiple Foo (call from `Foo` method)
        var commandProvider = new WildCocoCommandProvider(new[] { typeof(TestCommands_Multiple) });
        var commandCollectionRoot = commandProvider.GetCommandCollection();
        var commandStack = Array.Empty<CommandDescriptor>();

        var targetCommand = commandCollectionRoot.All.First(x => x.Name == "Foo");
        var commandCollection = commandCollectionRoot;

        var appContext = new WildCocoAppContext(targetCommand, default);
        appContext.Features.Set<IWildCocoCommandFeature>(new WildCocoCommandFeature(commandCollection, targetCommand, commandStack, new object() /* unused */));
        var helpBuilder = new WildCocoHelpMessageBuilder(
            new FakeAppContextAccessor(appContext),
            new FakeCommandHelpProvider(),
            new FakeHelpRenderer(),
            commandProvider
        );

        var help = helpBuilder.BuildAndRenderForCurrentCommand();
        help.Should().Be("Command;Name=Foo;SubCommandStack=");
    }


    [Fact]
    public void Nested_Index()
    {
        // $ ./app-nested 
        var commandProvider = new WildCocoCommandProvider(new[] { typeof(TestCommands_Nested) });
        var commandCollectionRoot = commandProvider.GetCommandCollection();
        var commandStack = Array.Empty<CommandDescriptor>();

        var targetCommand = BuiltInPrimaryCommand.GetCommand(string.Empty); // `TestCommands_Multiple` has no primary command. Use BuiltInPrimaryCommand.
        var commandCollection = commandCollectionRoot;

        var appContext = new WildCocoAppContext(targetCommand, default);
        appContext.Features.Set<IWildCocoCommandFeature>(new WildCocoCommandFeature(commandCollection, targetCommand, commandStack, new object() /* unused */));
        var helpBuilder = new WildCocoHelpMessageBuilder(
            new FakeAppContextAccessor(appContext),
            new FakeCommandHelpProvider(),
            new FakeHelpRenderer(),
            commandProvider
        );

        var help = helpBuilder.BuildAndRenderForCurrentContext();
        help.Should().Be("IndexHelp;Commands=Foo,Bar,TestCommands_Nested_1;SubCommandStack=");
    }

    [Fact]
    public void Nested_Context_Command()
    {
        // $ ./app-nested Foo --help
        var commandProvider = new WildCocoCommandProvider(new[] { typeof(TestCommands_Nested) });
        var commandCollectionRoot = commandProvider.GetCommandCollection();
        var commandStack = new[] { commandCollectionRoot.All.First(x => x.Name == "Foo") }; // --help (OptionLikeCommand) pushes the command to the stack.

        var targetCommand = BuiltInOptionLikeCommands.Help.Command;
        var commandCollection = commandCollectionRoot;

        var appContext = new WildCocoAppContext(targetCommand, default);
        appContext.Features.Set<IWildCocoCommandFeature>(new WildCocoCommandFeature(commandCollection, targetCommand, commandStack, new object() /* unused */));
        var helpBuilder = new WildCocoHelpMessageBuilder(
            new FakeAppContextAccessor(appContext),
            new FakeCommandHelpProvider(),
            new FakeHelpRenderer(),
            commandProvider
        );

        var help = helpBuilder.BuildAndRenderForCurrentContext();
        help.Should().Be("Command;Name=Foo;SubCommandStack=");
    }

    [Fact]
    public void Nested_Context_Command_Nested()
    {
        // $ ./app-nested TestCommands_Nested_1 Baz --help
        var commandProvider = new WildCocoCommandProvider(new[] { typeof(TestCommands_Nested) });
        var commandCollectionRoot = commandProvider.GetCommandCollection();
        var commandNested = commandCollectionRoot.All.First(x => x.Name == "TestCommands_Nested_1");
        var commandBaz = commandNested.SubCommands.All.First(x => x.Name == "Baz");
        var commandStack = new[] { commandNested, commandBaz }; // --help (OptionLikeCommand) pushes the command to the stack.

        var targetCommand = BuiltInOptionLikeCommands.Help.Command;
        var commandCollection = commandNested.SubCommands;

        var appContext = new WildCocoAppContext(targetCommand, default);
        appContext.Features.Set<IWildCocoCommandFeature>(new WildCocoCommandFeature(commandCollection, targetCommand, commandStack, new object() /* unused */));
        var helpBuilder = new WildCocoHelpMessageBuilder(
            new FakeAppContextAccessor(appContext),
            new FakeCommandHelpProvider(),
            new FakeHelpRenderer(),
            commandProvider
        );

        var help = helpBuilder.BuildAndRenderForCurrentContext();
        help.Should().Be("Command;Name=Baz;SubCommandStack=TestCommands_Nested_1");
    }


    private class TestCommands_Single
    {
        public void Foo(bool option0, [Argument]string args) { }
    }

    private class TestCommands_Multiple
    {
        public void Foo(bool option0, [Argument] string args) { }
        public void Bar(bool option0, [Argument] string args) { }
    }

    [HasSubCommands(typeof(TestCommands_Nested_1))]
    private class TestCommands_Nested
    {
        public void Foo(bool option0, [Argument] string args) { }
        public void Bar(bool option0, [Argument] string args) { }
    }
    private class TestCommands_Nested_1
    {
        public void Baz(bool option0, [Argument] string args) { }
        public void Hauhau(bool option0, [Argument] string args) { }
    }

    private class FakeAppContextAccessor : IWildCocoAppContextAccessor
    {
        public WildCocoAppContext? Current { get; set; }

        public FakeAppContextAccessor(WildCocoAppContext? context)
        {
            Current = context;
        }
    }

    private class FakeCommandHelpProvider : IWildCocoCommandHelpProvider
    {
        public HelpMessage CreateCommandHelp(CommandDescriptor command, IReadOnlyList<CommandDescriptor> subCommandStack)
            => new HelpMessage(new HelpSection(
                new HelpParagraph("Command"),
                new HelpParagraph("Name=" + command.Name),
                new HelpParagraph("SubCommandStack=" + string.Join(",", subCommandStack.Select(x => x.Name)))
            ));

        public HelpMessage CreateCommandsIndexHelp(CommandCollection commandCollection, IReadOnlyList<CommandDescriptor> subCommandStack)
            => new HelpMessage(new HelpSection(
                new HelpParagraph("IndexHelp"),
                new HelpParagraph("Commands=" + string.Join(",", commandCollection.All.Select(x => x.Name))),
                new HelpParagraph("SubCommandStack=" + string.Join(",", subCommandStack.Select(x => x.Name)))
            ));

        public HelpMessage CreateVersionHelp()
            => new HelpMessage(new HelpSection(
                new HelpParagraph("Version")
            ));
    }

    private class FakeHelpRenderer : IWildCocoHelpRenderer
    {
        public string Render(HelpMessage message)
        {
            return string.Join(";", message.Children[0].Children.OfType<HelpParagraph>().Select(x => x.Content));
        }
    }
}
