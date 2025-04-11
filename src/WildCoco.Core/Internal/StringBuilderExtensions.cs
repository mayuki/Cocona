using System.Text;

namespace WildCoco.Internal;

internal static class StringBuilderExtensions
{
    public static StringBuilder AppendPadding(this StringBuilder sb, int indentCount, string indent = "  ")
    {
        for (var i = 0; i < indentCount; i++) sb.Append(indent);
        return sb;
    }
}
