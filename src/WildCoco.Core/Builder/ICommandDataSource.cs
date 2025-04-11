namespace WildCoco.Builder;

/// <summary>
/// Defines a class that provides the mechanism to build a command data.
/// </summary>
public interface ICommandDataSource
{
    ICommandData Build();
}
