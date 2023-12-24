using System.Text;

namespace wDNS.Common.Extensions;

public static class Buffers
{
    public static ushort ReadUInt16(this byte[] buffer, ref int ptr)
    {
        var x = (ushort)(buffer[ptr++] << 8);
        x |= buffer[ptr++];

        return x;
    }

    public static void WriteUInt16(this byte[] buffer, ushort value, ref int ptr)
    {
        const byte mask = 0b11111111;

        buffer[ptr++] = (byte)(value >> 8);
        buffer[ptr++] = (byte)(value & mask);
    }

    public static void WriteUInt32(this byte[] buffer, uint value, ref int ptr)
    {
        const byte mask = 0b11111111;

        buffer[ptr++] = (byte)(value >> 24 & mask);
        buffer[ptr++] = (byte)(value >> 16 & mask);
        buffer[ptr++] = (byte)(value >> 8 & mask);
        buffer[ptr++] = (byte)(value & mask);
    }

    public static uint ReadUInt32(this byte[] buffer, ref int ptr)
    {
        var x = (uint)(buffer[ptr++] << 24);
        
        x |= (uint)(buffer[ptr++] << 16);
        x |= (uint)(buffer[ptr++] << 8);
        x |= buffer[ptr++];

        return x;
    }

    public static void WriteArray(this byte[] buffer, byte[] array, ref int ptr)
    {
        array.CopyTo(buffer, ptr);
        ptr += array.Length;
    }

    public static byte[] ReadArray(this byte[] buffer, int length, ref int ptr)
    {
        var read = new byte[length];
        Buffer.BlockCopy(buffer, ptr, read, 0, length);

        ptr += length;
        return read;
    }

    public delegate T MakeMethod<T>(byte[] buffer, ref int ptr);
    public static T[] ReadMany<T>(this byte[] buffer, MakeMethod<T> factory, int count, ref int ptr)
    {
        var items = new T[count];

        for (int i = 0; i < count; i++)
        {
            items[i] = factory(buffer, ref ptr);
        }

        return items;
    }

    public static void Write<T>(this IList<T> items, byte[] buffer, ref int ptr) where T : IBufferWritable
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Write(buffer, ref ptr);
        }
    }

    public static string Tox2String(this byte[] buffer) => ToX2String(buffer, 0, buffer.Length);

    public static string ToX2String(this byte[] buffer, int index, int length)
    {
        var sb = new StringBuilder();

        for (var i = index; i < length; i++)
        {
            sb.AppendFormat("{0} ", buffer[i].ToString("x2"));
        }

        return sb.ToString();
    }
}
