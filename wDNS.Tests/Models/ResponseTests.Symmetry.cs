using System.Buffers;
using wDNS.Common.Extensions;
using wDNS.Common.Models;

namespace wDNS.Tests.Models;

[TestClass]
public class ResponseSymmetryTests
{
    private record DelegatePair(Delegate Read, Delegate Write);

    [DataTestMethod]
    [DataRow(new object[] { new byte[]
    {
        // Message
        0x00, 0x01,         // Identification: 1
        0x00, 0x00,         // Flags: 0
        0x00, 0x01,         // Question Count: 1
        0x00, 0x02,         // Answer Count: 2
        0x00, 0x00,         // Authority Count: 0
        0x00, 0x00,         // Additional Count: 0

        // Question
            // QName
/*b12*/ 0x06,               // QName Length: 6
            // foobar
        0x66, 0x6F, 0x6F, 0x62, 0x61, 0x72,
/*b19*/ 0x03,               // QName Length: 3
            // com
/*b20*/ 0x63, 0x6F, 0x6D,
/*b23*/ 0x00,               // Label End
/*b24*/ 0x00, 0x01,         // QType: A
        0x00, 0x01,         // QClass: IN

        // Answers
        0xC0, 0x0C,         // QName Pointer: 12
        0x00, 0x01,         // QType: A
        0x00, 0x01,         // QClass: IN

            // TTL: 255
        0x00, 0x00, 0x00, 0xFF,

        0x00, 0x04,         // Data Length: 4
            // Data: 192.99.17.96
        0xC0, 0x63, 0x11, 0x60,

        // Answers
        0xC0, 0x0C,         // QName Pointer: 12
        0x00, 0x01,         // QType: A
        0x00, 0x01,         // QClass: IN

            // TTL: 255
        0x00, 0x00, 0x00, 0xFF,

        0x00, 0x04,         // Data Length: 4
            // Data: 192.99.17.3
        0xC0, 0x63, 0x11, 0x03
    } }, DisplayName = "Manual Buffer")]
    public void StepByStepSymmetric(byte[] rBuffer)
    {
        var uint16 = new DelegatePair(Buffers.ReadUInt16, Buffers.WriteUInt16);
        var uint32 = new DelegatePair(Buffers.ReadUInt32, Buffers.WriteUInt32);
        var label = new DelegatePair(DnsName.Read, delegate (byte[] buffer, DnsName name, ref int ptr) 
        {
            name.Write(buffer, ref ptr);
        });
        var array = new DelegatePair(
            delegate (byte[] buffer, ref int ptr)
            {
                var length = Buffers.ReadUInt16(buffer, ref ptr);
                return Buffers.ReadArray(buffer, length, ref ptr);
            }, 
            delegate (byte[] buffer, byte[] array, ref int ptr)
            {
                buffer.WriteUInt16((ushort) array.Length, ref ptr);
                buffer.WriteArray(array, ref ptr);
            });

        var steps = new DelegatePair[]
        {
            uint16,
            uint16,
            uint16,
            uint16, 
            uint16,
            uint16,

            label,
            uint16,
            uint16,

            label,
            uint16,
            uint16,

            uint32,
            array,

            label,
            uint16,
            uint16,

            uint32,
            array
        };


        var wBuffer = new byte[rBuffer.Length];

        var rParameters = new object[] { rBuffer, 0 };
        var wParameters = new object?[] { wBuffer, null, 0 };

        for (int i = 0; i < steps.Length; i++)
        {
            var readMethod = steps[i].Read.Method.Name;
            var writeMethod = steps[i].Write.Method.Name;

            object? value = steps[i].Read.DynamicInvoke(rParameters);
            wParameters[1] = value;

            steps[i].Write.DynamicInvoke(wParameters);

            Assert.AreEqual(rParameters[1], wParameters[2], $"Pointers are not at the same location at step {i}.");
        }
    }
}
