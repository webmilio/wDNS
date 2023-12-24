using System.Text;

namespace wDNS.Common.Helpers;

public class StringHelpers
{
    public static void Concatenate<T>(StringBuilder sb, IList<T> children)
    {
        for (int i = 0; i < children.Count; i++)
        {
            sb.Append('{');
            sb.Append(children[i]);
            sb.Append('}');

            if (i + 1 < children.Count)
            {
                sb.Append(", ");
            }
        }
    }

    public static string Concatenate<T>(IList<T> children)
    {
        var sb = new StringBuilder();
        Concatenate(sb, children);

        return sb.ToString();
    }
}
