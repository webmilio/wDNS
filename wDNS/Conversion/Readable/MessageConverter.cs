using System.Text;
using wDNS.Common.Models;

namespace wDNS.Conversion.Readable;

public interface IMessageConverter : IReadableStringConverter<Message>;

public class MessageConverter : IMessageConverter
{
    public void AppendReadableString(StringBuilder dst, Message instance, int spacing)
    {
        dst.AppendLineSpaced(spacing, $"Id: {instance.id}, Flags: {instance.flags}, Q/Ans/Auth/Add: {instance.questionCount}/{instance.answerCount}/{instance.authorityCount}/{instance.additionalCount}");
    }
}
