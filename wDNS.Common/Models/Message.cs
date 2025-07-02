using System;

namespace wDNS.Common.Models;

public struct Message : IBufferWritable, IBufferReadable, IEquatable<Message>
{
    public ushort id;
    public MessageFlags flags;
    public ushort questionCount;
    public ushort answerCount;
    public ushort authorityCount;
    public ushort additionalCount;

    public bool Response
    {
        readonly get => flags.HasFlag(MessageFlags.Query_Response);
        set => MessageFlagsHelpers.SetFlag(ref flags, MessageFlags.Query_Response, value);
    }

    public bool Truncated
    {
        readonly get => flags.HasFlag(MessageFlags.Truncation_Truncated);
        set => MessageFlagsHelpers.SetFlag(ref flags, MessageFlags.Truncation_Truncated, value);
    }

    public bool RecursionDesired
    {
        readonly get => flags.HasFlag(MessageFlags.RecursionDesired_Desired);
        set => MessageFlagsHelpers.SetFlag(ref flags, MessageFlags.RecursionDesired_Desired, value);
    }

    public bool RecursionSupported
    {
        readonly get => flags.HasFlag(MessageFlags.RecursionAvailable_Supported);
        set => MessageFlagsHelpers.SetFlag(ref flags, MessageFlags.RecursionAvailable_Supported, value);
    }

    public void Read(BufferContext context)
    {
        id = context.ReadUInt16();
        flags = (MessageFlags)context.ReadUInt16();

        questionCount = context.ReadUInt16();
        answerCount = context.ReadUInt16();
        authorityCount = context.ReadUInt16();
        additionalCount = context.ReadUInt16();
    }

    public readonly void Write(BufferContext context)
    {
        context.WriteUInt16(id);
        context.WriteUInt16((ushort)flags);

        context.WriteUInt16(questionCount);
        context.WriteUInt16(answerCount);
        context.WriteUInt16(authorityCount);
        context.WriteUInt16(additionalCount);
    }

    public override readonly string ToString() => $"ID: {id}, Flags: {flags}, Ques./Answ./Auth./Add.: {questionCount}/{answerCount}/{authorityCount}/{additionalCount}";

    public override readonly bool Equals(object? obj) => obj is Message message && Equals(message);

    public readonly bool Equals(Message other) => id == other.id &&
               flags == other.flags &&
               questionCount == other.questionCount &&
               answerCount == other.answerCount &&
               authorityCount == other.authorityCount &&
               additionalCount == other.additionalCount;

    public override readonly int GetHashCode() => HashCode.Combine(id, flags, questionCount, answerCount, authorityCount, additionalCount);

    public static bool operator ==(Message left, Message right) => left.Equals(right);

    public static bool operator !=(Message left, Message right) => !(left == right);
}
