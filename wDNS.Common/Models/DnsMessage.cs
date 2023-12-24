using System.Runtime.CompilerServices;
using wDNS.Common.Extensions;

namespace wDNS.Common.Models;

public struct DnsMessage : IBufferWritable, IBufferReadable<DnsMessage>
{
    public ushort identification;
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

    public void Write(byte[] buffer, ref int ptr)
    {
        buffer.WriteUInt16(identification, ref ptr);
        buffer.WriteUInt16((ushort)flags, ref ptr);

        buffer.WriteUInt16(questionCount, ref ptr);
        buffer.WriteUInt16(answerCount, ref ptr);
        buffer.WriteUInt16(authorityCount, ref ptr);
        buffer.WriteUInt16(additionalCount, ref ptr);
    }

    public static DnsMessage Read(byte[] buffer, ref int ptr)
    {
        var identification = buffer.ReadUInt16(ref ptr);
        var flags = (MessageFlags)buffer.ReadUInt16(ref ptr);

        var qdCount = buffer.ReadUInt16(ref ptr);
        var anCount = buffer.ReadUInt16(ref ptr);
        var nsCount = buffer.ReadUInt16(ref ptr);
        var arCount = buffer.ReadUInt16(ref ptr);

        return new()
        {
            identification = identification,
            flags = flags,

            questionCount = qdCount,
            answerCount = anCount,
            authorityCount = nsCount,
            additionalCount = arCount,
        };
    }

    public override string ToString()
    {
        return $"ID: {identification}, Flags: {flags}, Ques./Answ./Auth./Add.: {questionCount}/{answerCount}/{authorityCount}/{additionalCount}";
    }
}
