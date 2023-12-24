using System.Net;
using wDNS.Common.Helpers;

namespace wDNS.Configuration;

public class Forwarding
{
    public int Port { get; set; }

    public string[] Remotes { get; set; }

    public int Timeout { get; set; }

    public bool PrintQueryBytesOnReceiveError { get; set; }
    public bool PrintResponseBytesOnReceive { get; set; }
    public bool PrintResponseBytesOnReceiveError { get; set; }

    public IPEndPoint[] GetRemotes()
    {
        var remotes = new IPEndPoint[Remotes.Length];

        for (int i = 0; i < Remotes.Length; i++)
        {
            var parts = Remotes[i].Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            var address = NetworkHelpers.ParseIPAddress(parts[0]);
            var port = 53;

            if (parts.Length > 1)
            {
                port = int.Parse(parts[1]);
            }

            remotes[i] = new IPEndPoint(address, port);
        }

        return remotes;
    }
}
