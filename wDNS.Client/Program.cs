using System.Net;
using System.Net.Sockets;

namespace wDNS.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        byte[] dnsRequest = new byte[]
        {
            // DNS Message
            148, 99,    // Transaction ID (37987)
            1, 0,       // Flags (b0000_0001_0000_0000)
            0, 1,       // Question Count (1)
            0, 0,       // Answer Count (0)
            0, 0,       // Authority Count (0)
            0, 1,       // Additional Count (1)

            // Question
            7,          // QName Start
                        // Label length: 7
            101,        // e
            120,        // x
            97,         // a
            109,        // m
            112,        // p
            108,        // l
            101,        // e
            3,          // Label length: 3
            99,         // c
            111,        // o
            109,        // m
            0,          // QName Termination
            0, 1,       // QType: A
            0, 1        // QClass: IN
        };

        var dst = new IPEndPoint(IPAddress.Loopback, 53);
        using var client = new UdpClient(5566);
        
        client.Send(dnsRequest, dst);
        var response = client.Receive(ref dst);

        for (int i = 0; i < response.Length; i++)
        {
            Console.Write("{0} ", response[i].ToString("X2"));
        }
        Console.WriteLine();
    }
}
