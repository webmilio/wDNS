using System.Text;

namespace wDNS.Common.Models;

public struct LabelSegment : IBufferWritable, IBufferReadable
{
    public string value;
    public int length;

    public LabelSegment(string value)
    {
        this.value = value;
        this.length = value.Length;
    }

    public void Read(BufferContext context)
    {
        var length = context.ReadByte();
        var sb = new StringBuilder(length);

        // TODO: Length safety this, check for null termination.
        var max = context.pointer + length;

        for (;context.pointer < max && context.buffer[context.pointer] != Label.Terminator;)
        {
            var c = (char) context.ReadByte();
            sb.Append(c);
        }

        this.length = length;
        value = sb.ToString();
    }

    public readonly void Write(BufferContext context)
    {
        context.WriteByte((byte) length);
        
    }
}
