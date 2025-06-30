using System.Text;
using wDNS.Conversion.Readable;

namespace wDNS.Extensions;

public static class ReadableConverterExtensions
{
    public static string GetReadableString<T>(this IReadableStringConverter<T> converter, T instance)
    {
        var sb = new StringBuilder();
        converter.AppendReadableString(sb, instance, 0);

        return sb.ToString();
    }
}
