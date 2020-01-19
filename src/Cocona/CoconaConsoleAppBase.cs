using System;
using System.Collections.Generic;
using System.Text;

namespace Cocona
{
    /// <summary>
    /// An abstract class that implements for console application and provides some context.
    /// </summary>
    public abstract class CoconaConsoleAppBase
    {
        /// <summary>
        /// Gets a current application context.
        /// </summary>
        public CoconaAppContext Context { get; internal set; } = default!;
    }
}
