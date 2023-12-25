using wDNS.Common.Extensions;

namespace wDNS.Common.Models;

public struct Question : IBufferWritable, IBufferReadable<Question>
{
    public DnsName name;
    public RecordTypes type;
    public RecordClasses @class;

    public void Write(byte[] buffer, ref int ptr)
    {
        name.Write(buffer, ref ptr);
        buffer.WriteUInt16((ushort)type, ref ptr);
        buffer.WriteUInt16((ushort)@class, ref ptr);
    }

    public static Question Read(byte[] buffer, ref int ptr)
    {
        var label = DnsName.Read(buffer, ref ptr);

        var qType = (RecordTypes)buffer.ReadUInt16(ref ptr);
        var qClass = (RecordClasses)buffer.ReadUInt16(ref ptr);

        return new()
        {
            name = label,
            type = qType,
            @class = qClass
        };
    }

    public override string ToString()
    {
        return $"{type}/{@class}: {name}";
    }

    public override bool Equals(object? obj)
    {
        return obj is Question question &&
               name.Equals(question.name) &&
               type == question.type &&
               @class == question.@class;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(name, type, @class);
        //return HashCode.Combine(QName, QClass);
    }
}
