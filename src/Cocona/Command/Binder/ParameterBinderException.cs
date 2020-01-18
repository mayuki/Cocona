using System;

namespace Cocona.Command.Binder
{
    public class ParameterBinderException : Exception
    {
        public ParameterBinderResult Result { get; }
        public CommandArgumentDescriptor? Argument { get; }
        public CommandOptionDescriptor? Option { get; }

        public ParameterBinderException(ParameterBinderResult result, CommandOptionDescriptor? option = null, CommandArgumentDescriptor? argument = null, Exception? innerException = null)
            : this(result, result.ToString(), option, argument, innerException)
        {
        }

        public ParameterBinderException(ParameterBinderResult result, string message, CommandOptionDescriptor? option = null, CommandArgumentDescriptor? argument = null, Exception? innerException = null)
            : base(message, innerException)
        {
            Result = result;
            Option = option;
            Argument = argument;
        }
    }

    public enum ParameterBinderResult
    {
        None,
        InsufficientOption,
        InsufficientOptionValue,
        InsufficientArgument,
        MultipleArrayInArgument,
        TypeNotSupported,
    }
}
