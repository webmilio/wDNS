using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wDNS.Common.Models;

public struct Label : IBufferWritable, IBufferReadable, IEquatable<Label>
{
    public const int Terminator = 0x00;
    public const ushort Pointer = 0b1100_0000;

    public string[] segments;

    public Label()
    {
        segments = [];
    }

    public Label(params string[] segments)
    {
        this.segments = segments;
    }

    public void Read(BufferContext context) => this.segments = [.. ReadLabel(context)];

    private static IList<string> ReadLabel(BufferContext context)
    {
        var result = new List<string>();
        context = GetTargetBuffer(context);

        for (int i = 0; !context.EoB && context.CurrentByte != Terminator; i++)
        {
            var segment = ReadSegment(context);
            result.Add(segment);
        }

        context.Skip(); // Skip terminator

        return result;
    }

    private static string ReadSegment(BufferContext context)
    {
        // This is a normal label.
        var length = context.ReadByte();
        var segment = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            segment.Append(context.ReadChar());
        }

        return segment.ToString();
    }

    private static bool IsPointer(BufferContext context)
    {
        return (context.CurrentByte & Pointer) != 0;
    }

    private static BufferContext GetTargetBuffer(BufferContext context)
    {
        while (IsPointer(context))
        {
            var pointer = context.ReadPointer();
            context = new BufferContext(context.AsBuffer(), pointer);
        }

        return context;
    }

    /*public void Read(BufferContext context)
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
    }*/

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
