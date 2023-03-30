namespace Cocona.Builder;

internal class CommandDataDataSource : ICommandDataSource
{
    private readonly ICommandData _commandData;
    public CommandDataDataSource(ICommandData commandData)
    {
        _commandData = commandData;
    }
    public ICommandData Build() => _commandData;
}