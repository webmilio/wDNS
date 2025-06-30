using System.Text;
using wDNS.Common.Models;
using wDNS.Extensions;

namespace wDNS.Conversion.Readable;

public class AnswerConverter : IReadableStringConverter<Answer>
{
    public void AppendReadableString(StringBuilder dst, Answer instance, int spacing)
    {
        dst.AppendLineSpaced(spacing, $"TTL: {instance.ttl}");
        dst.AppendLineSpaced(spacing, $"Data: {instance.rdata.ToX2String()}");
    }
}
