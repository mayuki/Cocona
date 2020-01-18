using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona.Hosting
{
    /// <summary>
    /// Options for Cocona enabled application.
    /// </summary>
    public class CoconaAppOptions
    {
        /// <summary>
        /// If the type has public methods, Cocona treats as a command. The default value is true.
        /// </summary>
        public bool TreatPublicMethodsAsCommands { get; set; } = true;

        /// <summary>
        /// Gets a list of command types.
        /// </summary>
        public IList<Type> CommandTypes { get; set; } = new List<Type>();
    }
}
