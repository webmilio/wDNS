using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wDNS.Common.Models;

public struct Label : IBufferWritable, IBufferReadable, IEquatable<Label>
{
    public const int Terminator = 0x00;
    public const int Pointer = 0b1100_0000;

    public string[] segments;

    public Label()
    {
        segments = [];
    }

    public Label(params string[] segments)
    {
        this.segments = segments;
    }

    public void Read(BufferContext context)
    {
        var segments = new List<string>();

        Read(context, context.pointer, segments, out var offset);
        context.MovePointer(offset);

        this.segments = [.. segments];
    }

    private readonly void Read(BufferContext context, int offset, List<string> segments, out int headOffset)
    {
        headOffset = 1;

        for (; !context.EoB && context.CurrentByte != Terminator;)
        {
            if (TryGetPointer(context.buffer, offset, out var pointer))
            {
                // Pointers are two bytes in length (unsigned 16-bits int).

                headOffset = 2;
                Read(context, pointer, segments, out _);
            }
            else
            {
                context = new BufferContext(context.buffer, offset);
                
                var segment = ReadSegment(context);
                segments.Add(segment);

                offset = context.pointer;
            }
        }
    }

    private static string ReadSegment(BufferContext context)
    {
        // This is a normal label.
        var length = context.ReadByte();
        var segment = new StringBuilder(length);

        for (int i = 0; i < length && context.CurrentByte != Terminator; i++)
        {
            segment.Append(context.ReadChar());
        }

        return segment.ToString();
    }

    public readonly void Write(BufferContext destination)
    {
        for (int i = 0; i < segments.Length; i++)
        {
            destination.WriteByte((byte)segments[i].Length);

            for (int j = 0; j < segments[i].Length; j++)
            {
                destination.WriteChar(segments[i][j]);
            }
        }

        destination.WriteByte(Terminator);
    }

    /*public void Read(BufferContext context)
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
    }*/

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

    public static bool TryGetPointer(byte[] array, int offset, out ushort pointer)
    {
        const int CounterMask = ~Pointer;
        pointer = 0;

        if (offset + 1 >= array.Length || (array[offset] & Pointer) != Pointer)
        {
            return false;
        }
     
        // This is not super fun because it doesn't use the 'Read' methods that move pointers.
        // TODO Maybe figure out a safer way to do this.
        var a = (ushort)(array[offset] & CounterMask) << 8;
        var b = (ushort)(array[offset + 1]);

        pointer = (ushort)(a | b);
        return true;
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
