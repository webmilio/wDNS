using System.Text;
using wDNS.Common.Extensions;
using wDNS.Common.Helpers;

namespace wDNS.Common.Models;

public class Query : IBufferWritable
{
    public delegate void Delegate(object sender, Query query);
    public delegate void OnReadDelegate(object sender, byte[] buffer, int length, Query query);

    public DnsMessage Message { get; set; }
    public IList<Question> Questions { get; set; }

    public void Write(byte[] buffer, ref int ptr)
    {
        Message.Write(buffer, ref ptr);
        Questions.Write(buffer, ref ptr);
    }

    public static Query Read(byte[] buffer, ref int ptr)
    {
        var message = DnsMessage.Read(buffer, ref ptr);
        var questions = new Question[message.QuestionCount];

        for (int i = 0; i < questions.Length; i++)
        {
            questions[i] = Question.Read(buffer, ref ptr);
        }

        return new()
        {
            Message = message,
            Questions = questions
        };
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Message: {Message}");

        sb.Append("Questions: ");
        StringHelpers.Concatenate(sb, Questions);

        return sb.ToString();
    }
}
