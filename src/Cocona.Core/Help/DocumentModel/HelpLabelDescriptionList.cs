using System.Diagnostics;

namespace Cocona.Help.DocumentModel
{
    [DebuggerDisplay("LabelDescriptionList: Items={Items.Count,nq}")]
    public class HelpLabelDescriptionList : ICoconaHelpFlowContent
    {
        public List<HelpLabelDescriptionListItem> Items { get; }

        public HelpLabelDescriptionList(params HelpLabelDescriptionListItem[] items)
        {
            Items = new List<HelpLabelDescriptionListItem>(items ?? throw new ArgumentNullException(nameof(items)));
        }
    }

    [DebuggerDisplay("LabelDescriptionListItem: {Label,nq}: {Description,nq}")]
    public class HelpLabelDescriptionListItem
    {
        public string Label { get; }
        public string Description { get; }

        public HelpLabelDescriptionListItem(string label, string description)
        {
            Label = label ?? throw new ArgumentNullException(nameof(label));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
