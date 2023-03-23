using System;

namespace Cocona
{
    /// <summary>
    /// Specifies a class has a nested sub-commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HasSubCommandsAttribute : Attribute
    {
        /// <summary>
        ///  Gets the sub-commands collection type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///  Gets the sub-commands name.
        /// </summary>
        public string? CommandName { get; }

        /// <summary>
        ///  Gets or sets the sub-commands description.
        /// </summary>
        public string? Description { get; set; }

        public HasSubCommandsAttribute(Type commandsType, string? commandName = null)
        {
            Type = commandsType ?? throw new ArgumentNullException(nameof(commandsType));
            CommandName = commandName;
        }
    }

}
