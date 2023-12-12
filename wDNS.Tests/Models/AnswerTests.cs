using wDNS.Models;

namespace wDNS.Tests.Models;

[TestClass]
public class AnswerTests
{
    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] {
            0x07, 0x65, 0x78, 0x61, 0x6D, 0x70, 0x6C, 0x65, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x1C, 0x00, 0x01, 0x00, 0x00, 0x22, 0xB8, 0x00, 0x10, 0x20, 0x01, 0x0D, 0xB8, 0x85, 0xA3, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x2E, 0x03, 0x70, 0x73, 0x34
        },
        "example.com", QTypes.AAAA, QClasses.IN, 8888, 16,
        new byte[] { 0x20, 0x01, 0x0D, 0xB8, 0x85, 0xA3, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x2E, 0x03, 0x70, 0x73, 0x34 }
    })]

    public void Receive(byte[] buffer, string qName, QTypes qType, QClasses qClass, int ttl, int rdLength, byte[] rData)
    {
        int ptr = 0;
        var answer = Answer.Read(buffer, ref ptr);

        Equal(answer, qName, qType, qClass, ttl, rdLength, rData);
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
