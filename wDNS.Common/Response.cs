using System.Text;
using wDNS.Common.Extensions;

namespace wDNS.Common;

public class Response : IBufferWritable
{
    public delegate void FromQuestionDelegate(object sender, Query src, Response result);

    public DNSMessage Message { get; set; }
    public IList<Question> Questions { get; set; }
    public IList<Answer> Answers { get; set; }
    public object[] Authorities { get; set; }
    public object[] Additional { get; set; }

    public void Write(byte[] buffer, ref int ptr)
    {
        Message.Write(buffer, ref ptr);
        Questions.Write(buffer, ref ptr);
        Answers.Write(buffer, ref ptr);
    }

    public static Response Read(byte[] buffer, ref int ptr)
    {
        var message = DNSMessage.Read(buffer, ref ptr);
        var questions = buffer.ReadMany(Question.Read, message.QuestionCount, ref ptr);
        var answers = buffer.ReadMany(Answer.Read, message.AnswerCount, ref ptr);
        var authorities = new object[message.AuthorityCount];
        var additional = new object[message.AdditionalCount];

        return new()
        {
            Message = message,
            Questions = questions,
            Answers = answers,
            Authorities = authorities,
            Additional = additional
        };
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Message: {Message}");
        
        sb.Append("Questions: ");
        Concatenate(sb, Questions);

        sb.Append("Answers: ");
        Concatenate(sb, Answers);

        sb.Append("Authorities: ");
        Concatenate(sb, Authorities);

        sb.Append("Additional: ");
        Concatenate(sb, Additional);

        return sb.ToString();
    }

    private static void Concatenate<T>(StringBuilder sb, IList<T> children)
    {
        for (int i = 0; i < children.Count; i++)
        {
            sb.Append(children[i]);
            
            if (i + 1 < children.Count)
            {
                sb.Append(", ");
            }
        }

        sb.AppendLine();
    }
}
