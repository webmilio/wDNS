using System.Runtime.CompilerServices;
using System.Text;

namespace wDNS.Conversion.Readable;

public static class ConverterHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SpaceString(int spacing, string str)
    {
        return $"{new string(' ', spacing)}{str}";
    }

    public static StringBuilder AppendLineSpaced(this StringBuilder sb, int space, string str)
    {
        return sb.AppendLine(SpaceString(space, str));
    }
}
