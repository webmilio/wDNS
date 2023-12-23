namespace wDNS.Common;

public interface IBufferReadable<T>
{
    public static abstract T Read(byte[] buffer, ref int ptr);
}
