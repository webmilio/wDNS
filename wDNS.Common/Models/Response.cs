using System.Text;
using wDNS.Common.Extensions;
using wDNS.Common.Helpers;

namespace wDNS.Common.Models;

public struct Response : IBufferWritable, IBufferReadable<Response>
{
    public delegate void FromQuestionDelegate(object sender, Request src, byte[] buffer, Response result);

    public Request query;
    public IList<Answer> answers;
    public object[] authorities;
    public object[] additional;

    public Response(Request query, IList<Answer> answers, object[] authorities, object[] additional)
    {
        this.query = query;
        this.answers = answers;
        this.authorities = authorities;
        this.additional = additional;
    }

    public void Write(byte[] buffer, ref int ptr)
    {
        query.Write(buffer, ref ptr);
        answers.Write(buffer, ref ptr);
    }

    public static Response Read(byte[] buffer, ref int ptr)
    {
        var query = Request.Read(buffer, ref ptr);
        var answers = buffer.ReadMany(Answer.Read, query.message.answerCount, ref ptr);
        var authorities = new object[query.message.authorityCount];
        var additional = new object[query.message.additionalCount];

        return new(query, answers, authorities, additional);
    }

    public override string ToString()
    {
        const string spacer = "\n\t";
        var sb = new StringBuilder();

        sb.AppendLine($"Message: {query.message}");

        sb.Append("Questions: ");
        StringHelpers.Concatenate(sb, query.questions);
        sb.Append(spacer);

        sb.Append("Answers: ");
        StringHelpers.Concatenate(sb, answers);
        sb.Append(spacer);

        sb.Append("Authorities: ");
        StringHelpers.Concatenate(sb, authorities);
        sb.Append(spacer);

        sb.Append("Additional: ");
        StringHelpers.Concatenate(sb, additional);
        sb.Append(spacer);

        return sb.ToString();
    }
}
