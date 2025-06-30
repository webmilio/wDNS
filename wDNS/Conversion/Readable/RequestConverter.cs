using System.Text;
using wDNS.Common.Models;

namespace wDNS.Conversion.Readable;

public interface IRequestConverter : IReadableStringConverter<Request>;

public class RequestConverter(IReadableStringConverter<Message> message, IReadableStringConverter<Question> question) : IRequestConverter
{
    public void AppendReadableString(StringBuilder dst, Request instance, int spacing)
    {
        message.AppendReadableString(dst, instance.message, spacing);
        question.AppendReadableString(dst, instance.question, spacing);
    }
}
