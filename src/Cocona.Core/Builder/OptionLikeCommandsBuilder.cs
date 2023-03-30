namespace Cocona.Builder;

public interface IOptionLikeCommandsBuilder
{
    IOptionLikeCommandsBuilder Add(OptionLikeDelegateCommandDataSource commandDataSource);
}

public class OptionLikeCommandsBuilder : IOptionLikeCommandsBuilder
{
    private readonly List<OptionLikeDelegateCommandDataSource> _commandDataSources = new();
    private readonly Dictionary<string, object?> _properties = new();

    internal const string PropertyKeyFilters = "Cocona.Builder.OptionLikeCommandsBuilder+Filters";

    public IDictionary<string, object?> Properties => _properties;

    IOptionLikeCommandsBuilder IOptionLikeCommandsBuilder.Add(OptionLikeDelegateCommandDataSource commandDataSource)
    {
        _commandDataSources.Add(commandDataSource);
        return this;
    }

    public IReadOnlyList<OptionLikeDelegateCommandData> Build()
    {
        var commandDataItems = new List<OptionLikeDelegateCommandData>(_commandDataSources.Count);

        foreach (var commandDataSource in _commandDataSources)
        {
            var commandData = (OptionLikeDelegateCommandData)commandDataSource.Build();
            commandDataItems.Add(commandData);
        }

        return commandDataItems;
    }
}
