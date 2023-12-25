using System.Text;
using wDNS.Common.Extensions;
using wDNS.Common.Helpers;

namespace wDNS.Common.Models;

public struct Request : IBufferWritable, IBufferReadable<Request>
{
    public delegate void Delegate(object sender, Request query);
    public delegate void OnReadDelegate(object sender, byte[] buffer, Request query);

    public DnsMessage message;
    public IList<Question> questions;

    public void Write(byte[] buffer, ref int ptr)
    {
        message.Write(buffer, ref ptr);
        questions.Write(buffer, ref ptr);
    }

    public static Request Read(byte[] buffer, ref int ptr)
    {
        var message = DnsMessage.Read(buffer, ref ptr);
        var questions = buffer.ReadMany(Question.Read, message.questionCount, ref ptr);

        return new()
        {
            message = message,
            questions = questions
        };
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Message: {message}");

        sb.Append("Questions: ");
        StringHelpers.Concatenate(sb, questions);

        return sb.ToString();
    }
}
