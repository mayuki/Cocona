using WildCoco.Help.DocumentModel;
using WildCoco.Internal;
using System.Text;

namespace WildCoco.Help;

/// <summary>
/// A default implementation of help message renderer.
/// </summary>
public class WildCocoHelpRenderer : IWildCocoHelpRenderer
{
    public string Render(HelpMessage message)
    {
        var ctx = new WildCocoHelpRenderingContext();
        RenderContent(ctx, message);
        return ctx.StringBuilder.ToString();
    }

    protected virtual void RenderContent(WildCocoHelpRenderingContext ctx, IWildCocoHelpContent content)
    {
        switch (content)
        {
            case IWildCocoHelpSectioningContent section:
                RenderSectioningContent(ctx, section);
                break;
            case HelpHeading heading:
                RenderHeading(ctx, heading);
                break;
            case HelpParagraph paragraph:
                RenderParagraph(ctx, paragraph);
                break;
            case HelpLabelDescriptionList labelDescriptionList:
                RenderLabelDescriptionList(ctx, labelDescriptionList);
                break;
            case HelpPreformattedText preformattedText:
                RenderPreformattedText(ctx, preformattedText);
                break;
        }
    }

    protected virtual void RenderSectioningContent(WildCocoHelpRenderingContext ctx, IWildCocoHelpSectioningContent section)
    {
        using (new WildCocoHelpRenderingContext.SectionStackItem(ctx, section))
        {
            var prevContentIsSection = false;
            foreach (var child in section.Children)
            {
                if (prevContentIsSection)
                {
                    ctx.StringBuilder.AppendLine();
                }

                RenderContent(ctx, child);
                ctx.CurrentSection!.Advance();

                if (child is IWildCocoHelpSectioningContent)
                {
                    prevContentIsSection = true;
                }
            }
        }
    }

    protected virtual void RenderHeading(WildCocoHelpRenderingContext ctx, HelpHeading heading)
    {
        var depth = ctx.CurrentDepth - 1;
        ctx.StringBuilder
            .AppendPadding(depth)
            .AppendLine(heading.Content);
    }

    protected virtual void RenderParagraph(WildCocoHelpRenderingContext ctx, HelpParagraph paragraph)
    {
        var depth = ctx.CurrentDepth - 1;
        ctx.StringBuilder
            .AppendPadding(depth)
            .AppendLine(paragraph.Content);
    }

    protected virtual void RenderLabelDescriptionList(WildCocoHelpRenderingContext ctx, HelpLabelDescriptionList labelDescriptionList)
    {
        var depth = ctx.CurrentDepth - 1;
        var maxLabelLen = labelDescriptionList.Items.Max(x => x.Label.Length);
        foreach (var labelAndDesc in labelDescriptionList.Items)
        {
            ctx.StringBuilder
                .AppendPadding(depth)
                .Append(labelAndDesc.Label)
                .AppendPadding(maxLabelLen - labelAndDesc.Label.Length, " ")
                .Append("    ")
                .AppendLine(labelAndDesc.Description);
        }
    }

    protected virtual void RenderPreformattedText(WildCocoHelpRenderingContext ctx, HelpPreformattedText pre)
    {
        var depth = ctx.CurrentDepth - 1;

        foreach (var line in pre.Content.Split('\n'))
        {
            ctx.StringBuilder
                .AppendPadding(depth)
                .AppendLine(line.TrimEnd());
        }
    }

    public class WildCocoHelpRenderingContext
    {
        public int CurrentDepth => Sections.Count - 1; // (root) -> section ...
        public Stack<SectionStackItem> Sections { get; } = new Stack<SectionStackItem>();
        public SectionStackItem? CurrentSection { get; set; }
        public StringBuilder StringBuilder { get; } = new StringBuilder();

        public class SectionStackItem : IDisposable
        {
            private readonly WildCocoHelpRenderingContext _ctx;

            public IWildCocoHelpSectioningContent Section { get; }
            public int ChildPosition { get; private set; }

            public SectionStackItem(WildCocoHelpRenderingContext ctx, IWildCocoHelpSectioningContent section)
            {
                _ctx = ctx;
                _ctx.Sections.Push(this);
                _ctx.CurrentSection = this;

                Section = section;
                ChildPosition = 0;
            }

            public void Advance() => ChildPosition++;

            public void Dispose()
            {
                _ctx.Sections.Pop();
                _ctx.CurrentSection = _ctx.Sections.Count != 0 ? _ctx.Sections.Peek() : null;
            }
        }
    }
}
