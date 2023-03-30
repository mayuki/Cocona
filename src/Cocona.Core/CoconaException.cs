namespace Cocona
{
    public class CoconaException : Exception
    {
        public CoconaException(string message)
            : base(message)
        {
        }

        public CoconaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
