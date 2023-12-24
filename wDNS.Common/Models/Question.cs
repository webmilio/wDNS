using wDNS.Common.Extensions;

namespace wDNS.Common.Models;

public class Question : IBufferWritable, IBufferReadable<Question>
{
    public DnsName QName { get; set; }
    public RecordTypes QType { get; set; }
    public RecordClasses QClass { get; set; }

    public void Write(byte[] buffer, ref int ptr)
    {
        QName.Write(buffer, ref ptr);
        buffer.WriteUInt16((ushort)QType, ref ptr);
        buffer.WriteUInt16((ushort)QClass, ref ptr);
    }

    public static Question Read(byte[] buffer, ref int ptr)
    {
        var label = DnsName.Read(buffer, ref ptr);

        var qType = (RecordTypes)buffer.ReadUInt16(ref ptr);
        var qClass = (RecordClasses)buffer.ReadUInt16(ref ptr);

        return new()
        {
            QName = label,
            QType = qType,
            QClass = qClass
        };
    }

    public override string ToString()
    {
        return $"{QType}/{QClass}: {QName}";
    }

    public override bool Equals(object? obj)
    {
        return obj is Question question &&
               QName.Equals(question.QName) &&
               QType == question.QType &&
               QClass == question.QClass;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(QName, QType, QClass);
        //return HashCode.Combine(QName, QClass);
    }
}
