namespace wDNS.Models;

public interface IBufferWritable
{
    public void Write(byte[] buffer, ref int ptr);
}
