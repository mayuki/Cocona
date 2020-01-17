using System;
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

        [TransformHelp(typeof(SampleHelpTransformer))]
        public void Run()
        {
        }
    }

    class SampleHelpTransformer : ICoconaHelpTransformer
    {
        public void TransformHelp(HelpMessage helpMessage, CommandDescriptor command)
        {
            helpMessage.Children.Add(new HelpSection(
                new HelpHeading("Example:"),
                new HelpSection(
                    new HelpParagraph("MyApp --foo --bar")
                )
            ));
        }
    }
}
