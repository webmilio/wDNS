namespace wDNS.Common;

public interface IDnsName : IBufferWritable
{
    public bool Match(string name);
    public bool Match(byte[] data);

    public string Name { get; }
}
