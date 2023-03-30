namespace Cocona.Builder;

/// <summary>
/// Defines a class that provides the mechanism to configure application commands.
/// </summary>
public interface ICoconaCommandsBuilder
{
    ICoconaCommandsBuilder Add(ICommandDataSource commandDataSource);
    IDictionary<string, object?> Properties { get; }
    ICoconaCommandsBuilder New();
    IReadOnlyList<ICommandData> Build();
}

public class CoconaCommandsBuilder : ICoconaCommandsBuilder
{
    private readonly List<ICommandDataSource> _commandDataSources = new();
    private readonly Dictionary<string, object?> _properties = new();

    internal const string PropertyKeyFilters = "Cocona.Builder.CoconaCommandsBuilder+Filters";

    public IDictionary<string, object?> Properties => _properties;

    public CoconaCommandsBuilder()
    {
    }

    private CoconaCommandsBuilder(ICoconaCommandsBuilder parent)
    {
        // Copy properties to a new builder.
        foreach (var keyValue in parent.Properties)
        {
            // Copy filters to new properties bag.
            if (keyValue.Key == CoconaCommandsBuilder.PropertyKeyFilters && keyValue.Value is IList<object> filters)
            {
                _properties[keyValue.Key] = new List<object>(filters);
            }
            else
            {
                _properties[keyValue.Key] = keyValue.Value;
            }
        }
    }

    ICoconaCommandsBuilder ICoconaCommandsBuilder.New()
        => new CoconaCommandsBuilder(this);

    ICoconaCommandsBuilder ICoconaCommandsBuilder.Add(ICommandDataSource commandDataSource)
    {
        _commandDataSources.Add(commandDataSource);
        return this;
    }

    public IReadOnlyList<ICommandData> Build()
    {
        var commandDataItems = new List<ICommandData>(_commandDataSources.Count);

        foreach (var commandDataSource in _commandDataSources)
        {
            var commandData = commandDataSource.Build();
            commandDataItems.Add(commandData);
        }

        return commandDataItems;
    }
}
