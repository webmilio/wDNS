using System.Text;

namespace wDNS.Common;

public class Helpers
{
    public static void Concatenate<T>(StringBuilder sb, IList<T> children)
    {
        for (int i = 0; i < children.Count; i++)
        {
            sb.Append('{');
            sb.Append(children[i]);
            sb.Append('}');

            if (i + 1 < children.Count)
            {
                sb.Append(", ");
            }
        }
    }

    public static string Concatenate<T>(IList<T> children)
    {
        var sb = new StringBuilder();
        Concatenate(sb, children);

        return sb.ToString();
    }

    public static byte[] WriteBuffer(IBufferWritable obj)
    {
        int ptr = 0;
        var buffer = new byte[Constants.MaxUdpSize];

        obj.Write(buffer, ref ptr);
        Array.Resize(ref buffer, ptr);

        return buffer;
    }

    public static T ReadBuffer<T>(byte[] buffer, int startIndex = 0) where T : IBufferReadable<T>
    {
        int ptr = startIndex;
        var obj = T.Read(buffer, ref ptr);

        return obj;
    }
}
