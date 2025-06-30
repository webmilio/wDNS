using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace wDNS.Common.Helpers;

public class NetworkHelpers
{
    private delegate IPAddress IPAddressVariableLookup(string ipString);
    private static readonly Dictionary<string, IPAddressVariableLookup> _ipVariables = new(StringComparer.OrdinalIgnoreCase)
    {
        {
            "default-gateway",
            s => GetDefaultGateway() ?? throw new ArgumentException("Could not find default gateway!")
        }
    };

    public static IPAddress ParseIPAddress(string ipString)
    {
        if (ipString.StartsWith('$'))
        {
            var sub = ipString[1..];

            if (_ipVariables.TryGetValue(sub, out var func))
            {
                return func(sub);
            }
        }

        return IPAddress.Parse(ipString);
    }

    // Heavily inspired by (if not copied from) https://stackoverflow.com/questions/13634868/get-the-default-gateway
    public static IPAddress? GetDefaultGateway()
    {
        foreach (var intr in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (intr.OperationalStatus != OperationalStatus.Up ||
                intr.NetworkInterfaceType != NetworkInterfaceType.Ethernet ||
                intr.NetworkInterfaceType == NetworkInterfaceType.Loopback)
            {
                continue;
            }

            var ipProperties = intr?.GetIPProperties()?
                .GatewayAddresses?.FirstOrDefault(a => a != null)?
                .Address;

            if (ipProperties != null)
            {
                return ipProperties;
            }
        }

        return null;
    }
}
