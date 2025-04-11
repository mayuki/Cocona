namespace WildCoco.Application;

public interface IWildCocoAppContextAccessor
{
    WildCocoAppContext? Current { get; set; }
}

public class WildCocoAppContextAccessor : IWildCocoAppContextAccessor
{
    public WildCocoAppContext? Current { get; set; }
}
