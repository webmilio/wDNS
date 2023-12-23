using wDNS.Common;
using wDNS.Common.Models;

namespace wDNS.Tests.Models;

[TestClass]
public class DnsNameTests
{
    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] { 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0xFF, 0xFF },
        0, new byte[] { 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00 }
    })]
    [DataRow(new object[]
    {
        new byte[] { 0x00, 0x00, 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0xC0, 0x02, 0xFF, 0xFF },
        12,
        new byte[] { 0xC0, 0x02 }
    }, DisplayName = "Pointer")]
    public void ReadBufferCompareData(byte[] buffer, int startIndex = 0, byte[]? expected = null)
    {
        var dnsName = Helpers.ReadUdpBuffer<DnsName>(buffer, startIndex);
        CollectionAssert.AreEqual(expected ?? buffer, dnsName.data);
    }

    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] { 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00 },
        0, "bing.com"
    })]
    [DataRow(new object[]
    {
        new byte[] { 0x00, 0x00, 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0xC0, 0x02, 0xFF, 0xFF },
        12, "bing.com"
    }, DisplayName = "Pointer")]
    public void ReadBufferCompareName(byte[] buffer, int startIndex, string expected)
    {
        var dnsName = Helpers.ReadUdpBuffer<DnsName>(buffer, startIndex);
        Assert.AreEqual(expected, dnsName);
    }

    [DataTestMethod]
    [DataRow("bing.com", new byte[] { 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00 })]
    public void WriteNameCompareData(string name, byte[] expected)
    {
        var dnsName = new DnsName(name);
        var buffer = Helpers.WriteBuffer(dnsName);

        CollectionAssert.AreEqual(expected, buffer);
    }

    [DataTestMethod]
    [DataRow(new object[] { "bing.com" })]
    public void WriteNameCompareName(string name)
    {
        var dnsName = new DnsName(name);

        Assert.AreEqual(name, dnsName);
    }

    [DataTestMethod]
    [DataRow("bing.ca")]
    [DataRow("google.com")]
    [DataRow("audio-ak-spotify-com.akamaized.net")]
    public void Equals(string name)
    {
        var x = new DnsName(name);
        var y = new DnsName(name);

        Assert.AreEqual(x, y);
        Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
    }
}
