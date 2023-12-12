using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wDNS.Models;

namespace wDNS.Tests.Models;

[TestClass]
public class ResponseTests
{
    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] {
            0x77, 0x22, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x1C, 0x00, 0x01, 0xC0, 0x0C, 0x00, 0x1C, 0x00, 0x01, 0x00, 0x00, 0x05, 0x8E, 0x00, 0x10, 0x26, 0x20, 0x01, 0xEC, 0x0C, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0xCC, 0x4F, 0xC5, 0xC8
        },
        "bing.com", QTypes.AAAA, QClasses.IN, 1422, 16,
        new byte[] { 0x26, 0x20, 0x01, 0xEC, 0x0C, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00 }
    })]
    public void Receive(byte[] buffer, string qName, QTypes qType, QClasses qClass, int ttl, int rdLength, byte[] rData)
    {
        int ptr = 0;
        var response = Response.Read(buffer, ref ptr);

        Equal(response.Answers[0], qName, qType, qClass, ttl, rdLength, rData);
    }

    private void Equal(Answer answer, string qName, QTypes qType, QClasses qClass, int ttl, int rdLength, byte[] rData)
    {
        Assert.AreEqual(qName, answer.QName);
        Assert.AreEqual(qType, answer.Type);
        Assert.AreEqual(qClass, answer.Class);
        Assert.AreEqual((uint)ttl, answer.TTL);
        Assert.AreEqual(rdLength, answer.DataLength);
        Assert.AreEqual(rData.Length, answer.Data.Length);

        for (int i = 0; i < rData.Length; i++)
        {
            Assert.AreEqual(answer.Data[i], rData[i]);
        }
    }
}
