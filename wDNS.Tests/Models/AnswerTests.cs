using wDNS.Common;
using wDNS.Common.Helpers;
using wDNS.Common.Models;

namespace wDNS.Tests.Models;

[TestClass]
public class AnswerTests
{
    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] { 0x07, 0x65, 0x78, 0x61, 0x6D, 0x70, 0x6C, 0x65, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x1C, 0x00, 0x01, 0x00, 0x00, 0x22, 0xB8, 0x00, 0x10, 0x20, 0x01, 0x0D, 0xB8, 0x85, 0xA3, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x2E, 0x03, 0x70, 0x73, 0x34 },
        "example.com", RecordTypes.AAAA, RecordClasses.IN, 8888, 16,
        new byte[] { 0x20, 0x01, 0x0D, 0xB8, 0x85, 0xA3, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x2E, 0x03, 0x70, 0x73, 0x34 }
    })]

    public void Read(byte[] buffer, string qName, RecordTypes qType, RecordClasses qClass, int ttl, int rdLength, byte[] rData)
    {
        int ptr = 0;
        var answer = Answer.Read(buffer, ref ptr);

        Equal(answer, qName, qType, qClass, ttl, rdLength, rData);
    }

    [DataTestMethod]
    [DataRow(new object[]
    {
        "example.com", RecordTypes.AAAA, RecordClasses.IN, 8888,
        new byte[] { 0x20, 0x01, 0x0D, 0xB8, 0x85, 0xA3, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x2E, 0x03, 0x70, 0x73, 0x34 }
    })]
    public void Write(string qName, RecordTypes qType, RecordClasses qClass, int ttl, byte[] rData)
    {
        var question = new Question
        {
            QName = new(qName),
            QType = qType,
            QClass = qClass
        };

        var data = new AnswerData(qType, (ushort)rData.Length, rData);
        var answer = new Answer(question, data, (uint)ttl);

        var buffer = BufferHelpers.WriteBuffer(answer);

        Read(buffer, qName, qType, qClass, ttl, rData.Length, rData);
    }

    private void Equal(Answer answer, string qName, RecordTypes qType, RecordClasses qClass, int ttl, int rdLength, byte[] rData)
    {
        Assert.AreEqual(qName, answer.question.QName.name);
        Assert.AreEqual(qType, answer.question.QType);
        Assert.AreEqual(qClass, answer.question.QClass);
        Assert.AreEqual((uint)ttl, answer.ttl);
        Assert.AreEqual(rdLength, answer.data.length);
        Assert.AreEqual(rData.Length, answer.data.length);

        CollectionAssert.AreEqual(rData, answer.data.data);
    }
}
