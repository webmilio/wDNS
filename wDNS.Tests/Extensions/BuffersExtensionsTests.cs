using wDNS.Common;
using wDNS.Common.Extensions;
using wDNS.Common.Models;

namespace wDNS.Tests.Extensions;

[TestClass]
public class BuffersExtensionsTests
{
    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] { 0x04, 0x62, 0x69, 0x6E, 0x67, 0x00 },
        "bing"
    })]
    [DataRow(new object[]
    {
        new byte[] { 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00 },
        "bing.com"
    })]
    [DataRow(new object[]
    {
        new byte[] { 0x04, 0x6D, 0x61, 0x70, 0x73, 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00 },
        "maps.bing.com"
    })]
    public void ReadLabel(byte[] buffer, string expected)
    {
        int ptr = 0;
        var label = DnsName.Read(buffer, ref ptr);

        Assert.AreEqual(expected, label);
    }

    [DataTestMethod]
    [DataRow(new object[] { "bing.com" })]
    public void WriteLabel(string expected)
    {
        var buffer = new byte[Constants.MaxLabelsTotalLength];
        int ptr = 0;

        var name = new DnsName(expected);
        name.Write(buffer, ref ptr);

        Assert.AreEqual(10, ptr);

        Array.Resize(ref buffer, ptr);
        ptr = 0;

        var label = DnsName.Read(buffer, ref ptr);
        Assert.AreEqual(expected, label);
    }

    [DataTestMethod]
    [DataRow(new object[] { new byte[] { 0xC0, 0x0C }, 12 })]
    [DataRow(new object[] { new byte[] { 0b1111_1101, 0b1011_1111 }, 0b0011_1101_1011_1111 })]
    [DataRow(new object[] { new byte[] { 0x00, 0x0C }, 0 })]
    public void GetLabelPtr(byte[] buffer, int expected)
    {
        int ptr = 0;
        ptr = DnsName.GetLabelPointer(buffer, ref ptr);

        Assert.AreEqual(expected, ptr);
    }

    [DataTestMethod]
    [DataRow(new object[] { new byte[] { 0x29, 0x83 }, 10627 })]
    public void ReadUInt16(byte[] buffer, int expected)
    {
        int ptr = 0;
        var read = buffer.ReadUInt16(ref ptr);

        Assert.AreEqual((ushort)expected, read);
    }

    [DataTestMethod]
    [DataRow(new object[] { 10627 })]
    public void WriteUInt16(int expected)
    {
        int ptr = 0;
        var buffer = new byte[2];

        buffer.WriteUInt16((ushort)expected, ref ptr);
        ptr = 0;

        var read = buffer.ReadUInt16(ref ptr);
        Assert.AreEqual((ushort)expected, read);
    }

    [DataTestMethod]
    [DataRow(new object[] { new byte[] { 0x53, 0x29, 0xC7, 0x01 }, 1395246849 })]
    public void ReadUInt32(byte[] buffer, int expected)
    {
        int ptr = 0;
        var read = buffer.ReadUInt32(ref ptr);

        Assert.AreEqual((uint)expected, read);
    }

    [DataTestMethod]
    [DataRow(new object[] { 3260 })]
    public void WriteUInt32(int expected)
    {
        int ptr = 0;
        var buffer = new byte[4];

        buffer.WriteUInt32((uint)expected, ref ptr);
        ptr = 0;

        var read = buffer.ReadUInt32(ref ptr);
        Assert.AreEqual((uint)expected, read);
    }
}
