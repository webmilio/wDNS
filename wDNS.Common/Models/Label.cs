using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wDNS.Common.Models;

public struct Label : IBufferWritable, IBufferReadable, IEquatable<Label>
{
    public const int Terminator = 0x00;

    public const int Pointer = 0xC0;
    public const int Pointer2 = 0x0C;

    public LabelSegment[] segments;

    public Label()
    {
        segments = [];
    }

    public void Read(BufferContext context)
    {
        var segments = new List<LabelSegment>();

        for (; !context.EoB && context.CurrentByte != Terminator;)
        {

        }
    }

    public void ReadSeq(BufferContext context)
    {
        var segments = new List<LabelSegment>();

        for (;
            context.pointer < context.buffer.Length &&
            context.CurrentByte != Terminator;
        )
        {
            var segment = context.Read<LabelSegment>();

            if (segment.length == 0)
            {
                // Something went wrong, this shouldn't actually happen ever.
                // Advance so we don't get stuck in an infinite loop.
                context.pointer++;
            }
            else
            {
                segments.Add(segment);
            }
        }

        if (context.CurrentByte == Terminator)
        {
            context.ReadByte();
        }

        this.segments = [.. segments];
    }

    public readonly void Write(BufferContext context)
    {
        for (int i = 0; i < segments.Length; i++)
        {
            context.Write(segments[i]);
        }

        context.WriteByte(Terminator);
    }

    public override readonly string ToString()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < segments.Length; i++)
        {
            sb.Append(segments[i]);
            sb.Append('.');
        }

        return sb.ToString();
    }

    public override readonly bool Equals(object? obj) => obj is Label label && Equals(label);

    public readonly bool Equals(Label other) => Enumerable.SequenceEqual(segments, other.segments);

    public override readonly int GetHashCode()
    {
        int hashCode = 0;

        foreach (var s in segments)
        {
            hashCode = HashCode.Combine(hashCode, s);
        }

        return hashCode;
    }

    public static bool operator ==(Label left, Label right) => left.Equals(right);

    public static bool operator !=(Label left, Label right) => !(left == right);
}
