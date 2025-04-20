using System.Collections.Generic;

namespace wDNS.Common.Models;

public struct Label : IBufferWritable, IBufferReadable
{
    public const int Terminator = 0x00;

    public LabelSegment[] segments;

    public void Read(BufferContext context)
    {
        var segments = new List<LabelSegment>();

        for (;
            context.pointer < context.buffer.Length &&
            context.buffer[context.pointer] != Terminator;
            context.pointer++)
        {
            var segment = new LabelSegment();

            segment.Read(context);
            segments.Add(segment);
        }

        this.segments = [.. segments];
    }

    public readonly void Write(BufferContext context)
    {
        
    }
}
