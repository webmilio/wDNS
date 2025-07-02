using System;
using System.Linq;

namespace wDNS.Common.Models;

public struct Answer : IBufferReadable, IBufferWritable, IEquatable<Answer>
{
    public Label name;
    public RecordTypes type;
    public RecordClasses @class;

    public uint ttl;
    public ushort rdlength;
    public byte[] rdata;

    public void Read(BufferContext context)
    {
        name.Read(context);
        type = (RecordTypes)context.ReadUInt16();
        @class = (RecordClasses)context.ReadUInt16();

        ttl = context.ReadUInt32();
        rdlength = context.ReadUInt16();
        rdata = context.ReadArray(rdlength);
    }

    public readonly void Write(BufferContext context)
    {
        name.Write(context);
        context.WriteUInt16((ushort)type);
        context.WriteUInt16((ushort)@class);

        context.WriteUInt32(ttl);
        context.WriteUInt16(rdlength);
        context.WriteArray(rdata);
    }

    public override readonly bool Equals(object? obj) => obj is Answer data && Equals(data);

    public readonly bool Equals(Answer other) => name.Equals(other.name) &&
        type.Equals(other.type) && @class.Equals(other.@class) &&
        ttl == other.ttl &&
        rdlength == other.rdlength && rdata.SequenceEqual(other.rdata);

    public override readonly int GetHashCode()
    {
        var hash = HashCode.Combine(name, type, @class, ttl, rdlength);

        for (int i = 0; i < rdata.Length; i++)
        {
            hash = HashCode.Combine(hash, rdata[i]);
        }

        return hash;
    }

    public static bool operator ==(Answer left, Answer right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Answer left, Answer right)
    {
        return !(left == right);
    }
}
