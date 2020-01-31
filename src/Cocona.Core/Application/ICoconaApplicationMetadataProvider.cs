namespace Cocona.Application
{
    public interface ICoconaApplicationMetadataProvider
    {
        string GetExecutableName();
        string GetVersion();
        string GetProductName();
        string GetDescription();
    }
}
