using System.Collections.Generic;
using System.Text;
using Cocona.Help.DocumentModel;

namespace Cocona.Help
{
    /// <summary>
    /// Provides a help message based on the current context.
    /// </summary>
    public interface ICoconaHelpMessageBuilder
    {
        /// <summary>
        /// Build a help message based on the current context.
        /// </summary>
        /// <returns></returns>
        HelpMessage BuildForCurrentContext();

        /// <summary>
        /// Build a help message and render as string based on the current context.
        /// </summary>
        /// <returns></returns>
        string BuildAndRenderForCurrentContext();

        /// <summary>
        /// Build a help message based on the current command.
        /// </summary>
        /// <returns></returns>
        HelpMessage BuildForCurrentCommand();

        /// <summary>
        /// Build a help message and render as string based on the current command.
        /// </summary>
        /// <returns></returns>
        string BuildAndRenderForCurrentCommand();
    }
}
