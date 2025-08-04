using System.Text;
using wDNS.Common.Models;
using wDNS.Extensions;

namespace wDNS.Conversion.Readable;

public class AnswerConverter : IReadableStringConverter<Answer>
{
    public void AppendReadableString(StringBuilder dst, Answer instance, int spacing)
    {
        dst.AppendLineSpaced(spacing, $"TTL: {instance.ttl}");

        var printable = GetPrintable(instance);
        dst.AppendLineSpaced(spacing, $"Data: {printable}");
    }

    private string GetPrintable(Answer instance)
    {
        switch (instance.type)
        {
            case RecordTypes.A:
                return GetPrintableIPv4(instance);
            case RecordTypes.AAAA:
                return GetPrintableIPv6(instance);
            default:
                return instance.rdata.ToX2String();
        }
    }

    private static string GetPrintableIPv6(Answer instance)
    {
        var sb = new StringBuilder(instance.rdlength);
        for (int i = 0; i < instance.rdlength; i += 2)
        {
            var hex = Convert.ToHexStringLower(instance.rdata, i, 2);

            sb.Append(hex);
            sb.Append("::");
        }

        return sb.ToString();
    }

    private static string GetPrintableIPv4(Answer instance)
    {
        var sb = new StringBuilder(instance.rdlength);
        for (int i = 0; i < instance.rdlength; i++)
        {
            sb.Append(instance.rdata[i]);
            sb.Append('.');
        }

        return sb.ToString();
    }
}
