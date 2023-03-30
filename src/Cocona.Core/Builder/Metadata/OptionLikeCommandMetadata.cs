namespace Cocona.Builder.Metadata
{
    public class OptionLikeCommandMetadata : IOptionLikeCommandMetadata
    {
        private readonly ICommandData _commandData;

        public string OptionName { get; }
        public IReadOnlyList<char> ShortNames { get; }

        public OptionLikeCommandMetadata(OptionLikeDelegateCommandData commandData)
        {
            OptionName = commandData.Metadata.OfType<CommandNameMetadata>().LastOrDefault()?.Name ?? throw new InvalidOperationException("An option-like command must have a name.");
            ShortNames = commandData.ShortNames;
            _commandData = commandData;
        }

        public ICommandData GetCommandData() => _commandData;
    }

    public interface IOptionLikeCommandMetadata
    {
        string OptionName { get; }
        IReadOnlyList<char> ShortNames { get; }
        ICommandData GetCommandData();
    }
}
