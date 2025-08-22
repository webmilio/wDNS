using System;

namespace wDNS.Common.Models;

public struct LabelSegment : IBufferWritable, IBufferReadable, IEquatable<LabelSegment>
{
    public string value;
    public byte length;

    public LabelSegment(string value)
    {
        this.value = value;
        length = (byte) value.Length;
    }

    public void Read(BufferContext context)
    {
        // TODO: Length safety this, check for null termination.
        if (context.CurrentByte == Label.Pointer)
        {
            ReadPointer(context);
        }
        else
        {
            ReadLabel(context);
        }
    }

    private void ReadLabel(BufferContext context)
    {
        /*var start = context.pointer;
        var length = context.ReadByte();
        var max = context.pointer + length;

        var sb = new StringBuilder(length);

        for (; context.pointer < max && context.CurrentByte != Label.Terminator;)
        {
            var c = context.ReadChar();
            sb.Append(c);
        }

        this.length = length;
        value = sb.ToString();

        context.TryAddSegment((byte) start, this);*/
    }

    private void ReadPointer(BufferContext context)
    {
        /*_ = context.ReadByte(); // Move away from the Pointer byte onto the Address byte.
        var address = context.ReadByte();

        if (!context.TryGetSegment(address, out var segment))
        {
            throw new IndexOutOfRangeException("Label segment has invalid address.");
        }

        this = segment;*/
    }

    public readonly void Write(BufferContext context)
    {
        /*if (context.TryGetSegmentPointer(value, out var pointer))
        {
            context.WriteByte(Label.Pointer);
            context.WriteByte(pointer);
        }
        else
        {
            var start = pointer;

            context.WriteByte(length);
            for (int i = 0; i < length; i++)
            {
                context.WriteChar(value[i]);
            }

            context.TryAddSegment(this, start);
        }*/
    }

    public override readonly string ToString()
    {
        return value;
    }

    public override readonly bool Equals(object? obj) => obj is LabelSegment segment && Equals(segment);

    public readonly bool Equals(LabelSegment other) => value == other.value && length == other.length;

    public override readonly int GetHashCode() => HashCode.Combine(value, length);

    public static bool operator ==(LabelSegment left, LabelSegment right) => left.Equals(right);

    public static bool operator !=(LabelSegment left, LabelSegment right) => !(left == right);
}
