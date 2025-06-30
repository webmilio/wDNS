using System.Net;
using System.Text.Json.Serialization;
using wDNS.Common.Helpers;

namespace wDNS.Configuration;

#pragma warning disable IDE1006 // Naming Styles
public class Listening : ComponentConfiguration
{
    public string IPAddress { get; set; } = "0.0.0.0";
    internal IPAddress _IPAddress => NetworkHelpers.ParseIPAddress(IPAddress);

    public int Port { get; set; } = 53;

    public bool PrintBytesOnReceive { get; set; }

    public int? ReceiveBufferSize { get; set; }
}
#pragma warning restore IDE1006 // Naming Styles
