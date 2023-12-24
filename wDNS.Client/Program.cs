using System.Net;
using System.Net.Sockets;
using wDNS.Common.Models;

namespace wDNS.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        // example.com
        byte[] dnsRequest = new byte[]
        {
            // DNS Message
            148, 99,    // Transaction ID (37987)
            1, 0,       // Flags (b0000_0001_0000_0000)
            0, 1,       // Question Count (1)
            0, 0,       // Answer Count (0)
            0, 0,       // Authority Count (0)
            0, 0,       // Additional Count (0)

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
        /*var dnsRequest = new byte[]
        {
            0x91, 0x4C, 
            0x01, 0x00, 
            0x00, 0x01, 
            0x00, 0x00,
            0x00, 0x00, 
            0x00, 0x00, 
            
            0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x1C, 0x00, 0x01
        };*/

        // audio-akamai-cdn.akamaized.net
        // var dnsRequest = new byte[] { 0x88, 0x5E, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x14, 0x61, 0x75, 0x64, 0x69, 0x6F, 0x2D, 0x61, 0x6B, 0x2D, 0x73, 0x70, 0x6F, 0x74, 0x69, 0x66, 0x79, 0x2D, 0x63, 0x6F, 0x6D, 0x09, 0x61, 0x6B, 0x61, 0x6D, 0x61, 0x69, 0x7A, 0x65, 0x64, 0x03, 0x6E, 0x65, 0x74, 0x00, 0x00, 0x01, 0x00, 0x01 };
        
        var dst = new IPEndPoint(IPAddress.Loopback, 53);
        using var client = new UdpClient(5566);

        client.Send(dnsRequest, dst);
        var buffer = client.Receive(ref dst);

        for (int i = 0; i < buffer.Length; i++)
        {
            Console.Write("{0} ", buffer[i].ToString("X2"));
        }
        Console.WriteLine();

        var ptr = 0;
        var response = Response.Read(buffer, ref ptr);

        Console.WriteLine(response);
    }
}
