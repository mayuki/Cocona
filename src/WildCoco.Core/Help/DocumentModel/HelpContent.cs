namespace WildCoco.Help.DocumentModel;

public interface IWildCocoHelpContent
{
}

public interface IWildCocoHelpFlowContent : IWildCocoHelpContent
{
}

public interface IWildCocoHelpSectioningContent : IWildCocoHelpContent
{
    IEnumerable<IWildCocoHelpContent> Children { get; }
}
