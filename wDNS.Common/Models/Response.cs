using System;

namespace wDNS.Common.Models;

public struct Response : IBufferReadable, IBufferWritable, IEquatable<Response>
{
    public Message message;
    public Question question;
    public Answer[] data;

    public void Read(BufferContext context)
    {
        message.Read(context);
        question.Read(context);

        data = new Answer[message.answerCount];

        for (int i = 0; i < data.Length; i++)
        {
            data[i].Read(context);
        }
    }

    public readonly void Write(BufferContext context)
    {
        message.Write(context);
        question.Write(context);

        for (int i = 0; i < data.Length; i++)
        {
            data[i].Write(context);
        }
    }

    public override readonly bool Equals(object? obj) => obj is Response answer && Equals(answer);

    public readonly bool Equals(Response other) => message.Equals(other.question) && question.Equals(other.question) && data.Equals(other.data);

    public override readonly int GetHashCode() => HashCode.Combine(question, data);

    public static bool operator ==(Response left, Response right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Response left, Response right)
    {
        return !(left == right);
    }
}
