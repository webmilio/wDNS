using wDNS.Common.Extensions;

namespace wDNS.Common;

public class Question : IBufferWritable
{
    public string QName { get; set; }
    public QTypes QType { get; set; }
    public QClasses QClass { get; set; }

    public void Write(byte[] buffer, ref int ptr)
    {
        buffer.WriteLabel(QName, ref ptr);
        buffer.WriteUInt16((ushort)QType, ref ptr);
        buffer.WriteUInt16((ushort)QClass, ref ptr);
    }

    public static Question Read(byte[] buffer, ref int ptr)
    {
        var label = buffer.ReadLabel(ref ptr);

        var qType = (QTypes)buffer.ReadUInt16(ref ptr);
        var qClass = (QClasses)buffer.ReadUInt16(ref ptr);

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
               QName == question.QName &&
               QType == question.QType &&
               QClass == question.QClass;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(QName, QType, QClass);
    }
}
