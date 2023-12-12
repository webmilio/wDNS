using wDNS.Extensions;

namespace wDNS.Models;

public class Answer : IBufferWritable
{
    public string QName { get; set; }
    public QTypes Type { get; set; }
    public QClasses Class { get; set; }
    public uint TTL { get; set; }
    public ushort DataLength { get; set; }
    public byte[] Data { get; set; }

    public void Write(byte[] buffer, ref int ptr)
    {
        buffer.WriteLabel(QName, ref ptr);
        buffer.WriteUInt16((ushort)Type, ref ptr);
        buffer.WriteUInt16((ushort)Class, ref ptr);
        buffer.WriteUInt32(TTL, ref ptr);

        buffer.WriteUInt16(DataLength, ref ptr);
        buffer.WriteArray(Data, ref ptr);
    }

    public static Answer Read(byte[] buffer, ref int ptr)
    {
        var qName = buffer.ReadLabel(ref ptr);
        var type = (QTypes)buffer.ReadUInt16(ref ptr);
        var @class = (QClasses)buffer.ReadUInt16(ref ptr);
        var ttl = buffer.ReadUInt32(ref ptr);
        var dataLength = buffer.ReadUInt16(ref ptr);
        var data = buffer.ReadArray(dataLength, ref ptr);

        return new()
        {
            QName = qName,
            Type = type,
            Class = @class,
            TTL = ttl,
            DataLength = dataLength,
            Data = data
        };
    }

    public static Answer FromQuestion(Question question, uint ttl, ushort dataLength, byte[] data)
    {
        return new()
        {
            QName = question.QName,
            Type = question.QType,
            Class = question.QClass,

            TTL = ttl,
            DataLength = dataLength,
            Data = data
        };
    }
}
