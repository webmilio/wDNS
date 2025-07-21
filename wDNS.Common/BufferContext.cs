using System;
using System.Runtime.CompilerServices;

namespace wDNS.Common;

public partial class BufferContext : IBufferReadable, IBufferWritable
{
    public readonly byte[] buffer;

    /// <summary>Use this as a <c>ref</c>.</summary>
    public int pointer;

    public byte CurrentByte
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => buffer[pointer];
    }

    public bool EoB => pointer >= buffer.Length;

    public BufferContext(byte[] buffer)
    {
        this.buffer = buffer;
    }

    public BufferContext(byte[] buffer, int pointer) : this(buffer)
    {
        this.pointer = pointer;
    }

    public byte ReadByte() => buffer[pointer++];
    public byte Peek() => pointer >= buffer.Length ? (byte)0 : buffer[pointer + 1];

    public void WriteByte(byte value) => buffer[pointer++] = value;

    public char ReadChar() => (char)ReadByte();

    public void WriteChar(char value) => WriteByte((byte)value);

    public T Read<T>() where T : IBufferReadable, new()
    {
        var item = new T();
        item.Read(this);

        return item;
    }

    public void Write<T>(T item) where T : IBufferWritable => item.Write(this);

    public ushort ReadUInt16()
    {
        var x = (ushort)(ReadByte() << 8);
        x |= ReadByte();

        return x;
    }

    public void WriteUInt16(ushort value)
    {
        WriteByte((byte)(value >> 8));
        WriteByte((byte)value);
    }

    public void WriteUInt32(uint value)
    {
        WriteByte((byte)(value >> 24));
        WriteByte((byte)(value >> 16));
        WriteByte((byte)(value >> 8));
        WriteByte((byte)value);
    }

    public uint ReadUInt32()
    {
        var x = (uint)(ReadByte() << 24);
        x |= (uint)(ReadByte() << 16);
        x |= (uint)(ReadByte() << 8);
        x |= ReadByte();

        return x;
    }

    public byte[] ReadArray(int length)
    {
        var array = new byte[length];
        Buffer.BlockCopy(buffer, pointer, array, 0, array.Length);

        pointer += length;
        return array;
    }

    public void WriteArray(byte[] data)
    {
        Buffer.BlockCopy(data, 0, buffer, pointer, data.Length);
        pointer += data.Length;
    }

    public byte[] ToBuffer()
    {
        var buffer = new byte[pointer];
        Buffer.BlockCopy(this.buffer, 0, buffer, 0, buffer.Length);

        return buffer;
    }

    public void ResetPointer() => pointer = 0;

    public void MovePointer(int offset) => pointer += offset;

    public void Write(BufferContext write)
    {
        write.WriteArray(buffer);
    }

    public void Read(BufferContext context)
    {
        var buffer = context.ReadArray(context.buffer.Length);
        Buffer.BlockCopy(buffer, 0, this.buffer, pointer, buffer.Length);
    }

    public static BufferContext CreateUdpContext() => new(new byte[Constants.MaxUdpSize]);
}
