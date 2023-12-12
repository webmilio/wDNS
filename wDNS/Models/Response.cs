using wDNS.Extensions;

namespace wDNS.Models;

public class Response : IBufferWritable
{
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
        var message =       DNSMessage.Read(buffer, ref ptr);
        var questions =     buffer.ReadMany(Question.Read,      message.QuestionCount,      ref ptr);
        var answers =       buffer.ReadMany(Answer.Read,        message.AnswerCount,        ref ptr);
        var authorities =   new object[message.AuthorityCount];
        var additional =    new object[message.AdditionalCount];

        return new()
        {
            Message =       message,
            Questions =     questions,
            Answers =       answers,
            Authorities =   authorities,
            Additional =    additional
        };
    }
}
