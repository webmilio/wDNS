using System.Net;
using wDNS.Common.Extensions;

namespace wDNS.Common.Models;

public record AnswerData(QTypes QType, ushort Length, byte[] Data, object? VisualRepresentation = null) : IBufferWritable
{
    public void Write(byte[] buffer, ref int ptr)
    {
        buffer.WriteUInt16(Length, ref ptr);
        buffer.WriteArray(Data, ref ptr);
    }

    public static AnswerData Read(QTypes qType, byte[] buffer, ref int ptr)
    {
        var length = buffer.ReadUInt16(ref ptr);

        int mPtr = ptr;
        var data = buffer.ReadArray(length, ref ptr);

        object visualRepresentation;

        switch (qType)
        {
            case QTypes.A:
            case QTypes.AAAA:
                visualRepresentation = new IPAddress(data);
                break;
            case QTypes.CNAME:
                visualRepresentation = DnsName.Read(buffer, ref mPtr);
                break;
            default:
                visualRepresentation = "Not Implemented";
                break;
        }

        return new(qType, length, data, visualRepresentation);
    }

    public override string ToString()
    {
        return $"{QType}: {VisualRepresentation}";
    }
}
