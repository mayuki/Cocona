using Cocona;
using Cocona.Command;
using Cocona.Help;
using Cocona.Help.DocumentModel;

namespace CoconaSample.Advanced.HelpTransformer
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        [SampleTransformHelp]
        public void Run()
        {
            Console.WriteLine("To show help message, use '--help' option.");
        }
    }

    class SampleTransformHelpAttribute : TransformHelpAttribute
    {
        public override void TransformHelp(HelpMessage helpMessage, CommandDescriptor command)
        {
            var descSection = (HelpSection)helpMessage.Children.First(x => x is HelpSection section && section.Id == HelpSectionId.Description);
            descSection.Children.Add(new HelpPreformattedText(@"
  ________ 
 < Hello! >
  -------- 
         \   ^__^
          \  (oo)\_______
             (__)\       )\/\
                 ||----w |
                 ||     ||
"));

            helpMessage.Children.Add(new HelpSection(
                new HelpHeading("Example:"),
                new HelpSection(
                    new HelpParagraph("MyApp --foo --bar")
                )
            ));
        }
    }
}
