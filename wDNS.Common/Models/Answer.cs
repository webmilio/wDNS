using wDNS.Common.Extensions;

namespace wDNS.Common.Models;

public struct Answer : IBufferWritable, IBufferReadable<Answer>
{
    public readonly Question question;
    public readonly AnswerData data;

    public uint ttl;

    public Answer(Question question, AnswerData data, uint ttl)
    {
        this.question = question;
        this.data = data;

        this.ttl = ttl;
    }

    public uint TickTTL()
    {
        return --ttl;
    }

    public void Write(byte[] buffer, ref int ptr)
    {
        question.Write(buffer, ref ptr);
        buffer.WriteUInt32(ttl, ref ptr);

        data.Write(buffer, ref ptr);
    }

    public static Answer Read(byte[] buffer, ref int ptr)
    {
        var question = Question.Read(buffer, ref ptr);

        var ttl = buffer.ReadUInt32(ref ptr);
        var data = AnswerData.Read(question.QType, buffer, ref ptr);

        return new(question, data, ttl);
    }

    public override string ToString()
    {
        return $"TTL: {ttl} {data}";
    }
}
