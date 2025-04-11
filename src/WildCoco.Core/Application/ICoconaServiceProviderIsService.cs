namespace WildCoco.Application;

public interface IWildCocoServiceProviderIsService
{
    bool IsService(Type t);
}

public class NullWildCocoServiceProviderIsService : IWildCocoServiceProviderIsService
{
    public static IWildCocoServiceProviderIsService Instance { get; } = new NullWildCocoServiceProviderIsService();
    public bool IsService(Type t) => false;
}
