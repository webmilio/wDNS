using System.Collections.Generic;
using System.Collections.ObjectModel;
using wDNS.Common.Models;

namespace wDNS.Common;

public class BufferContext
{
    public readonly byte[] buffer;

    /// <summary>Use this as a <c>ref</c>.</summary>
    public int pointer;

    private readonly Dictionary<int, Label> _ptrLabels = [];
    private readonly Dictionary<Label, int> _labelPtrs = [];

    public BufferContext(byte[] buffer)
    {
        this.buffer = buffer;
    }

    public byte ReadByte() => buffer[pointer++];

    public void WriteByte(byte value) => buffer[pointer++] = value;

    public ushort ReadUInt16()
    {
        var x = (ushort)(ReadByte() << 8);
        x |= ReadByte();

        return x;
    }

    public void WriteUInt16(ushort value)
    {
        const byte mask = 0b11111111;

        buffer[pointer++] = (byte)(value >> 8);
        buffer[pointer++] = (byte)(value & mask);
    }

    public void WriteUInt32(uint value)
    {
        const byte mask = 0b11111111;

        buffer[pointer++] = (byte)(value >> 24 & mask);
        buffer[pointer++] = (byte)(value >> 16 & mask);
        buffer[pointer++] = (byte)(value >> 8 & mask);
        buffer[pointer++] = (byte)(value & mask);
    }

    public uint ReadUInt32()
    {
        var x = (uint)(ReadByte() << 24);

        x |= (uint)(ReadByte() << 16);
        x |= (uint)(ReadByte() << 8);
        x |= ReadByte();

        return x;
    }

    public void ResetPointer() => pointer = 0;
}
