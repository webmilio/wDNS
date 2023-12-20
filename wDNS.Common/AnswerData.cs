using System.Net;
using wDNS.Common.Extensions;

namespace wDNS.Common;

public record AnswerData(QTypes QType, ushort Length, byte[] Data) : IBufferWritable
{
    private IPAddress _address;
    public IPAddress Address => _address ??= new IPAddress(Data);

    public void Write(byte[] buffer, ref int ptr)
    {
        buffer.WriteUInt16(Length, ref ptr);
        buffer.WriteArray(Data, ref ptr);
    }

    public static AnswerData Read(QTypes qType, byte[] buffer, ref int ptr)
    {
        var length = buffer.ReadUInt16(ref ptr);
        var data = buffer.ReadArray(length, ref ptr);

        return new(qType, length, data);
    }

    public override string ToString()
    {
        return $"{QType}: {Address}";
    }
}
