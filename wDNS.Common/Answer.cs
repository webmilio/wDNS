using wDNS.Common.Extensions;

namespace wDNS.Common;

public class Answer : IBufferWritable
{
    public Question Question { get; init; }
    public uint TTL { get; set; }
    public AnswerData Data { get; init; }

    public void Write(byte[] buffer, ref int ptr)
    {
        Question.Write(buffer, ref ptr);
        buffer.WriteUInt32(TTL, ref ptr);

        Data.Write(buffer, ref ptr);
    }

    public static Answer Read(byte[] buffer, ref int ptr)
    {
        var question = Question.Read(buffer, ref ptr);

        var ttl = buffer.ReadUInt32(ref ptr);
        var data = AnswerData.Read(question.QType, buffer, ref ptr);

        return new()
        {
            Question = question,
            TTL = ttl,
            Data = data
        };
    }

    public override string ToString()
    {
        return $"TTL: {TTL}, {Data}";
    }
}
