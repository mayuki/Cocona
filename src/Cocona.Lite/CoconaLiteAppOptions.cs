using System;
using System.Collections.Generic;

namespace Cocona
{
    /// <summary>
    /// Options for Cocona enabled application.
    /// </summary>
    public class CoconaLiteAppOptions
    {
        /// <summary>
        /// If the type has public methods, Cocona treats as a command. The default value is true.
        /// </summary>
        public bool TreatPublicMethodsAsCommands { get; set; } = true;

        /// <summary>
        /// Specify enable to convert command name to lower-case or not (e.g. CommandName -> command-name)
        /// </summary>
        public bool EnableConvertCommandNameToLowerCase { get; set; } = true;

        /// <summary>
        /// Specify enable to convert option name to lower-case or not (e.g. OptionName -> option-name)
        /// </summary>
        public bool EnableConvertOptionNameToLowerCase { get; set; } = true;

        /// <summary>
        /// Specify enable shell completion support. The default value is true.
        /// </summary>
        public bool EnableShellCompletionSupport { get; set; } = true;

        /// <summary>
        /// Gets a list of command types.
        /// </summary>
        public IList<Type> CommandTypes { get; set; } = new List<Type>();
    }
}
