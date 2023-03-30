using Cocona.Help.DocumentModel;
using Cocona.Internal;
using System.Text;

namespace Cocona.Help;

/// <summary>
/// A default implementation of help message renderer.
/// </summary>
public class CoconaHelpRenderer : ICoconaHelpRenderer
{
    public string Render(HelpMessage message)
    {
        var ctx = new CoconaHelpRenderingContext();
        RenderContent(ctx, message);
        return ctx.StringBuilder.ToString();
    }

    protected virtual void RenderContent(CoconaHelpRenderingContext ctx, ICoconaHelpContent content)
    {
        switch (content)
        {
            case ICoconaHelpSectioningContent section:
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

    protected virtual void RenderSectioningContent(CoconaHelpRenderingContext ctx, ICoconaHelpSectioningContent section)
    {
        using (new CoconaHelpRenderingContext.SectionStackItem(ctx, section))
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

                if (child is ICoconaHelpSectioningContent)
                {
                    prevContentIsSection = true;
                }
            }
        }
    }

    protected virtual void RenderHeading(CoconaHelpRenderingContext ctx, HelpHeading heading)
    {
        var depth = ctx.CurrentDepth - 1;
        ctx.StringBuilder
            .AppendPadding(depth)
            .AppendLine(heading.Content);
    }

    protected virtual void RenderParagraph(CoconaHelpRenderingContext ctx, HelpParagraph paragraph)
    {
        var depth = ctx.CurrentDepth - 1;
        ctx.StringBuilder
            .AppendPadding(depth)
            .AppendLine(paragraph.Content);
    }

    protected virtual void RenderLabelDescriptionList(CoconaHelpRenderingContext ctx, HelpLabelDescriptionList labelDescriptionList)
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

    protected virtual void RenderPreformattedText(CoconaHelpRenderingContext ctx, HelpPreformattedText pre)
    {
        var depth = ctx.CurrentDepth - 1;

        foreach (var line in pre.Content.Split('\n'))
        {
            ctx.StringBuilder
                .AppendPadding(depth)
                .AppendLine(line.TrimEnd());
        }
    }

    public class CoconaHelpRenderingContext
    {
        public int CurrentDepth => Sections.Count - 1; // (root) -> section ...
        public Stack<SectionStackItem> Sections { get; } = new Stack<SectionStackItem>();
        public SectionStackItem? CurrentSection { get; set; }
        public StringBuilder StringBuilder { get; } = new StringBuilder();

        public class SectionStackItem : IDisposable
        {
            private readonly CoconaHelpRenderingContext _ctx;

            public ICoconaHelpSectioningContent Section { get; }
            public int ChildPosition { get; private set; }

            public SectionStackItem(CoconaHelpRenderingContext ctx, ICoconaHelpSectioningContent section)
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