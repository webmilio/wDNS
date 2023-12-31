﻿using System.Text;
using wDNS.Common.Extensions;

namespace wDNS.Common.Models;

public class DnsName : IBufferWritable, IBufferReadable<DnsName>, IDnsName
{
    public string Name { get; }
    public readonly byte[] data;

    public DnsName(string name) : this(name, Constants.MaxLabelsTotalLength)
    {
    }

    public DnsName(string name, int bufferLength)
    {
        this.Name = name;

        int ptr = 0;
        var buffer = new byte[bufferLength];

        WriteLabels(buffer, name, ref ptr);
        Array.Resize(ref buffer, ptr);

        this.data = buffer;
    }

    public DnsName(string name, byte[] data)
    {
        this.Name = name;
        this.data = data;
    }

    public void Write(byte[] buffer, ref int ptr)
    {
        if (data != null)
        {
            buffer.WriteArray(data, ref ptr);
        }
        else
        {
            WriteLabels(buffer, Name, ref ptr);
        }
    }

    public static void WriteLabels(byte[] buffer, string label, ref int ptr)
    {
        var split = label.Split('.');

        for (int i = 0; i < split.Length; i++)
        {
            var str = Encoding.ASCII.GetBytes(split[i]);

            buffer[ptr++] = (byte)str.Length;

            Buffer.BlockCopy(str, 0, buffer, ptr, str.Length);
            ptr += (byte)str.Length;
        }

        buffer[ptr++] = 0; // Labels end
    }

    public static DnsName Read(byte[] buffer, ref int ptr)
    {
        var start = ptr;
        var mPtr = GetLabelPointer(buffer, ref ptr);

        var label = new StringBuilder(Constants.MaxLabelSegmentLength);

        while (mPtr < buffer.Length && buffer[mPtr] > 0)
        {
            mPtr = GetLabelPointer(buffer, ref mPtr);
            ushort length = buffer[mPtr++];

            var end = mPtr + length;

            for (; mPtr < end;)
            {
                label.Append((char)buffer[mPtr++]);
            }

            label.Append('.');
        }

        mPtr++; // Skip the 0-byte at the end of the Name.
        label.Remove(label.Length - 1, 1); // Remove trailing dot. TODO: Add this in some config file.

        // Move passed pointer to new pointer head location if necessary
        ptr = Math.Max(ptr, mPtr);

        var data = new byte[ptr - start];
        Buffer.BlockCopy(buffer, start, data, 0, data.Length);

        return new DnsName(label.ToString(), data);
    }

    public static int GetLabelPointer(byte[] buffer, ref int ptr)
    {
        const int PtrMask = 0b0011_1111_1111_1111;

        if ((buffer[ptr] & 0b1100_0000) == 0b1100_0000)
        {
            var mPtr = buffer.ReadUInt16(ref ptr) & PtrMask;
            return mPtr;
        }

        return ptr;
    }

    public static implicit operator string(DnsName d) => d.Name;
    public static implicit operator byte[](DnsName d) => d.data;

    public virtual bool Match(string name)
    {
        return this.Name.Equals(name);
    }

    public virtual bool Match(byte[] data)
    {
        return Enumerable.SequenceEqual(this.data, data);
    }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is DnsName name && Match(name.data);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode(); // TODO This is most likely not a good solution to the dictionary problem. Change this.
    }
}
