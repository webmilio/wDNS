using System.Text;
using wDNS.Models;

namespace wDNS.Extensions;

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

    public static string ReadLabel(this byte[] buffer, ref int ptr)
    {
        const byte maxSegmentLength = 63;
        const byte maxLabelLength = 255;

        var label = new StringBuilder(maxLabelLength);
        int localPtr = buffer.GetLabelPointer(ref ptr); ;

        while (localPtr < buffer.Length && buffer[localPtr] != 0)
        {
            ushort wordLength = (ushort)(buffer[localPtr++] & maxSegmentLength);

            var wordEnd = localPtr + wordLength;
            for (; localPtr < wordEnd; localPtr++)
            {
                label.Append((char)buffer[localPtr]);
            }

            if (localPtr + 1 < buffer.Length && buffer[localPtr + 1] != 0)
            {
                label.Append('.');
            }
        }

        ptr = localPtr > ptr ? localPtr : ptr;
        ptr++;

        return label.ToString();
    }

    public static int GetLabelPointer(this byte[] buffer, ref int ptr)
    {
        if (buffer[ptr] == 0b11000000)
        {
            return buffer[++ptr];
        }

        return ptr;
    }

    public static void WriteLabel(this byte[] buffer, string label, ref int ptr)
    {
        var split = label.Split('.');

        for (int i = 0; i < split.Length; i++)
        {
            var str = Encoding.ASCII.GetBytes(split[i]);

            buffer[ptr++] = (byte)str.Length;

            System.Buffer.BlockCopy(str, 0, buffer, ptr, str.Length);
            ptr += (byte)str.Length;
        }

        buffer[ptr++] = 0;
    }

    public static void WriteArray(this byte[] buffer, byte[] array, ref int ptr)
    {
        array.CopyTo(buffer, ptr);
        ptr += array.Length;
    }

    public static byte[] ReadArray(this byte[] buffer, int length, ref int ptr)
    {
        var read = new byte[length];

        try
        {
            System.Buffer.BlockCopy(buffer, ptr, read, 0, length);
        }
        catch (Exception ex)
        {
            return new byte[length];
        }

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
}
