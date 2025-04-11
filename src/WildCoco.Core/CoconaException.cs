namespace WildCoco;

public class WildCocoException : Exception
{
    public WildCocoException(string message)
        : base(message)
    {
    }

    public WildCocoException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
