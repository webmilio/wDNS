using System.Text;
using wDNS.Common;

namespace wDNS.Extensions;

public static class SpanExtensions
{
    public static string ToX2String(this IList<byte> buffer) => ToX2String(buffer, 0, buffer.Count);

    public static string ToX2String(this IList<byte> buffer, int index, int length)
    {
        var sb = new StringBuilder();

        for (var i = index; i < length; i++)
        {
            sb.AppendFormat("{0} ", buffer[i].ToString("x2"));
        }

        return sb.ToString();
    }

    public static byte[] ToBuffer(this IBufferWritable writable) 
    {
        var ctx = BufferContext.CreateUdpContext();
        ctx.Write(writable);

        return ctx.AsBuffer();
    }
}
