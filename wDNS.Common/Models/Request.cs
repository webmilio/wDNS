using System;

namespace wDNS.Common.Models;

public struct Request : IBufferWritable, IBufferReadable, IEquatable<Request>
{
    public Message message;
    public Question question;

    public void ReadSeq(BufferContext context)
    {
        message.ReadSeq(context);
        question.ReadSeq(context);
    }

    public readonly void Write(BufferContext context)
    {
        message.Write(context);
        question.Write(context);
    }

    public override bool Equals(object? obj) => obj is Request request && Equals(request);

    public bool Equals(Request other) => message.Equals(other.message) &&
               question.Equals(other.question);

    public override readonly int GetHashCode() => HashCode.Combine(message, question);

    public static bool operator ==(Request left, Request right) => left.Equals(right);

    public static bool operator !=(Request left, Request right) => !(left == right);
}
