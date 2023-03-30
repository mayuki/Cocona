using Cocona;
using Cocona.Help;

namespace CoconaSample.Advanced.HelpOnDemand
{
    class Program
    {
        static void Main(string[] args)
        {
            CoconaApp.Run<Program>(args);
        }

        public void ForContext(bool optionA, [FromService] ICoconaHelpMessageBuilder helpMessageBuilder)
        {
            // Show commands help. (same as `./HelpOnDemand --help`)
            Console.WriteLine(helpMessageBuilder.BuildAndRenderForCurrentContext());
        }

        public void ForCommand(bool optionA, [FromService] ICoconaHelpMessageBuilder helpMessageBuilder)
        {
            // Show a help for this command. (same as `./HelpOnDemand for-command --help`)
            Console.WriteLine(helpMessageBuilder.BuildAndRenderForCurrentCommand());
        }
    }
}
