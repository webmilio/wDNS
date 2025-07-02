using System;

namespace wDNS.Common.Models;

public struct Question : IBufferWritable, IBufferReadable, IEquatable<Question>
{
    public Label name;
    public RecordTypes types;
    public RecordClasses @class;

    public void Read(BufferContext context)
    {
        name.Read(context);

        types = (RecordTypes)context.ReadUInt16();
        @class = (RecordClasses)context.ReadUInt16();
    }

    public readonly void Write(BufferContext destination)
    {
        name.Write(destination);

        destination.WriteUInt16((ushort)types);
        destination.WriteUInt16((ushort)@class);
    }

    public override readonly bool Equals(object? obj) => obj is Question question && Equals(question);

    public readonly bool Equals(Question other) => name.Equals(other.name) &&
               types == other.types &&
               @class == other.@class;

    public override readonly int GetHashCode() => HashCode.Combine(name, types, @class);

    public static bool operator ==(Question left, Question right) => left.Equals(right);

    public static bool operator !=(Question left, Question right) => !(left == right);
}
