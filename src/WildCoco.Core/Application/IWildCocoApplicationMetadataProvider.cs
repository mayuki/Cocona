namespace WildCoco.Application;

public interface IWildCocoApplicationMetadataProvider
{
    string GetExecutableName();
    string GetVersion();
    string GetProductName();
    string GetDescription();
}
