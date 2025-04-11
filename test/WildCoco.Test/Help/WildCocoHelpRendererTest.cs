using WildCoco.Help;
using WildCoco.Help.DocumentModel;
using WildCoco.Help;
using WildCoco.Help.DocumentModel;

namespace WildCoco.Test.Help;

public class WildCocoHelpRendererTest
{
    [Fact]
    public void RenderTest_Section()
    {
        var message = new HelpMessage(
            new HelpSection(
                new HelpParagraph("1st")
            )
        );

        new WildCocoHelpRenderer().Render(message).Should().Be(@"
1st
".TrimStart());
    }

    [Fact]
    public void RenderTest_Section_Spacing()
    {
        var message = new HelpMessage(
            new HelpSection(
                new HelpParagraph("1st")
            ),
            new HelpSection(
                new HelpParagraph("2nd")
            )
        );

        new WildCocoHelpRenderer().Render(message).Should().Be(@"
1st

2nd
".TrimStart());
    }

    [Fact]
    public void RenderTest_Section_Nested_Indent_NoSpacing()
    {
        var message = new HelpMessage(
            new HelpSection(
                new HelpHeading("1st")
            ),
            new HelpSection(
                new HelpHeading("2nd"),
                new HelpSection(
                    new HelpParagraph("3rd")
                )
            )
        );

        new WildCocoHelpRenderer().Render(message).Should().Be(@"
1st

2nd
  3rd
".TrimStart());
    }

    [Fact]
    public void RenderTest_Section_Nested_Indent_NoSpacing2()
    {
        var message = new HelpMessage(
            new HelpSection(
                new HelpParagraph("1st")
            ),
            new HelpSection(
                new HelpParagraph("2nd"),
                new HelpSection(
                    new HelpParagraph("3rd"),
                    new HelpParagraph("4th")
                )
            )
        );

        new WildCocoHelpRenderer().Render(message).Should().Be(@"
1st

2nd
  3rd
  4th
".TrimStart());
    }

    [Fact]
    public void RenderTest_LabelDescriptionList()
    {
        var message = new HelpMessage(
            new HelpSection(
                new HelpHeading("Usage: ConsoleApp1")
            ),
            new HelpSection(
                new HelpParagraph("description of an application")
            ),
            new HelpSection(
                new HelpHeading("Options:"),
                new HelpSection(
                    new HelpLabelDescriptionList(
                        new HelpLabelDescriptionListItem("--foo, -f", "Foo option (Required)"),
                        new HelpLabelDescriptionListItem("--looooooong-option, -l", "Long name option")
                    )
                )
            ),
            new HelpSection(
                new HelpHeading("Examples:"),
                new HelpSection(
                    new HelpParagraph("ConsoleApp1 --foo --bar")
                )
            )
        );

        new WildCocoHelpRenderer().Render(message).Should().Be(@"
Usage: ConsoleApp1

description of an application

Options:
  --foo, -f                  Foo option (Required)
  --looooooong-option, -l    Long name option

Examples:
  ConsoleApp1 --foo --bar
".TrimStart());
    }

    [Fact]
    public void RenderTest_Preformatted()
    {
        var message = new HelpMessage(
            new HelpSection(
                new HelpParagraph("preformatted text"),
                new HelpSection(
                    new HelpPreformattedText("using System;\r\n\r\nclass Program\r\n{\r\n    static void Main(string[] args)\r\n    {\r\n        Console.WriteLine(123);\r\n    }\r\n}")
                )
            )
        );

        new WildCocoHelpRenderer().Render(message).Should().Be(@"
preformatted text
  using System;
  
  class Program
  {
      static void Main(string[] args)
      {
          Console.WriteLine(123);
      }
  }
".TrimStart());
    }

}
