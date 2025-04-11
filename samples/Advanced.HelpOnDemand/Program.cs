using WildCoco;
using WildCoco.Help;
using WildCoco;
using WildCoco.Help;

namespace WildCocoSample.Advanced.HelpOnDemand;

class Program
{
    static void Main(string[] args)
    {
        WildCocoApp.Run<Program>(args);
    }

    public void ForContext(bool optionA, [FromService] IWildCocoHelpMessageBuilder helpMessageBuilder)
    {
        // Show commands help. (same as `./HelpOnDemand --help`)
        Console.WriteLine(helpMessageBuilder.BuildAndRenderForCurrentContext());
    }

    public void ForCommand(bool optionA, [FromService] IWildCocoHelpMessageBuilder helpMessageBuilder)
    {
        // Show a help for this command. (same as `./HelpOnDemand for-command --help`)
        Console.WriteLine(helpMessageBuilder.BuildAndRenderForCurrentCommand());
    }
}
