using System.Net;
using wDNS.Common.Extensions;

namespace wDNS.Common.Models;

public struct AnswerData : IBufferWritable
{
    public readonly RecordTypes type;
    public readonly ushort length;
    public readonly byte[] data;
    public object? visual;

    public AnswerData(RecordTypes type, ushort length, byte[] data) : this(type, length, data, null)
    {   
    }

    public AnswerData(RecordTypes type, ushort length, byte[] data, object? visual)
    {
        this.type = type;
        this.length = length;
        this.data = data;
        this.visual = visual;
    }

    public void Write(byte[] buffer, ref int ptr)
    {
        buffer.WriteUInt16(length, ref ptr);
        buffer.WriteArray(data, ref ptr);
    }

    public static AnswerData Read(RecordTypes qType, byte[] buffer, ref int ptr)
    {
        var length = buffer.ReadUInt16(ref ptr);

        int mPtr = ptr;
        var data = buffer.ReadArray(length, ref ptr);

        object visualRepresentation;

        switch (qType)
        {
            case RecordTypes.A:
            case RecordTypes.AAAA:
                visualRepresentation = new IPAddress(data);
                break;
            case RecordTypes.CNAME:
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
        return $"{type}: {visual}";
    }
}
