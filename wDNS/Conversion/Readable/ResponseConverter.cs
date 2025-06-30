using System.Text;
using wDNS.Common.Models;

namespace wDNS.Conversion.Readable;

public class ResponseConverter : IReadableStringConverter<Response>
{
    private readonly IReadableStringConverter<Question> _question;
    private readonly IReadableStringConverter<Answer> _answer;

    public void AppendReadableString(StringBuilder dst, Response instance, int spacing)
    {
        _question.AppendReadableString(dst, instance.question, spacing);

        for (int i = 0; i < instance.data.Length; i++)
        {
            _answer.AppendReadableString(dst, instance.data[i], spacing);
        }
    }
}
