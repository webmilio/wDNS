namespace wDNS.Common.Helpers;

public static class BufferHelpers
{

    public static T ReadBuffer<T>(byte[] buffer, int startIndex = 0) where T : IBufferReadable<T>
    {
        int ptr = startIndex;
        var obj = T.Read(buffer, ref ptr);

        return obj;
    }

    public static byte[] WriteBuffer(IBufferWritable obj)
    {
        int ptr = 0;
        var buffer = new byte[Constants.MaxUdpSize];

        obj.Write(buffer, ref ptr);
        Array.Resize(ref buffer, ptr);

        return buffer;
    }
}