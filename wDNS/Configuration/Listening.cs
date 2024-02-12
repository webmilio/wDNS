using System.Net;
using System.Text.Json.Serialization;
using wDNS.Common.Helpers;

namespace wDNS.Configuration;

#pragma warning disable IDE1006 // Naming Styles
public class Listening
{
    public string IPAddress { get; set; }
    internal IPAddress _IPAddress => NetworkHelpers.ParseIPAddress(IPAddress);

    public int Port { get; set; } = 53;

    public bool PrintBytesOnReceive { get; set; }

    [JsonPropertyName("Active")]
    public string[] ActiveClassNames { get; set; } = [$"{typeof(Listening).Namespace}.{typeof(Listening).Name}"];
}
#pragma warning restore IDE1006 // Naming Styles
