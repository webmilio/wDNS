using wDNS.Common.Extensions;

namespace wDNS.Common;

public class Query : IBufferWritable
{
    public delegate void Delegate(object sender, Query query);

    public DNSMessage Message { get; set; }
    public IList<Question> Questions { get; set; }

    public void Write(byte[] buffer, ref int ptr)
    {
        Message.Write(buffer, ref ptr);
        Questions.Write(buffer, ref ptr);
    }

    public static Query Read(byte[] buffer, ref int ptr)
    {
        var message = DNSMessage.Read(buffer, ref ptr);
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
}
