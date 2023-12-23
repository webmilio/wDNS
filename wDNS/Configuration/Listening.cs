using System.Net;

namespace wDNS.Configuration;

#pragma warning disable IDE1006 // Naming Styles
public class Listening
{
    public string IPAddress { get; set; }
    internal IPAddress _IPAddress => System.Net.IPAddress.Parse(IPAddress);

    public int Port { get; set; } = 53;

    public bool PrintBytesOnReceive { get; set; }
}
#pragma warning restore IDE1006 // Naming Styles
