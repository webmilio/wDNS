using System.Text;
using wDNS.Common.Extensions;

namespace wDNS.Common.Models;

public class DnsMessage : IBufferWritable, IBufferReadable<DnsMessage>
{
    public ushort Identification { get; set; }
    public MessageFlags Flags { get; set; }
    public ushort QuestionCount { get; set; }
    public ushort AnswerCount { get; set; }
    public ushort AuthorityCount { get; set; }
    public ushort AdditionalCount { get; set; }

    public void Write(byte[] buffer, ref int ptr)
    {
        buffer.WriteUInt16(Identification, ref ptr);
        buffer.WriteUInt16((ushort)Flags, ref ptr);

        buffer.WriteUInt16(QuestionCount, ref ptr);
        buffer.WriteUInt16(AnswerCount, ref ptr);
        buffer.WriteUInt16(AuthorityCount, ref ptr);
        buffer.WriteUInt16(AdditionalCount, ref ptr);
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
            Identification = identification,
            Flags = flags,

            QuestionCount = qdCount,
            AnswerCount = anCount,
            AuthorityCount = nsCount,
            AdditionalCount = arCount,
        };
    }

    public override string ToString()
    {
        return $"ID: {Identification}, Flags: {Flags}, Ques./Answ./Auth./Add.: {QuestionCount}/{AnswerCount}/{AuthorityCount}/{AdditionalCount}";
    }
}
