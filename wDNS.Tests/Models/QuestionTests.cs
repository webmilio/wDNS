using wDNS.Common;
using wDNS.Common.Helpers;
using wDNS.Common.Models;

namespace wDNS.Tests.Common;

[TestClass]
public class QuestionTests
{
    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] { 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x01, 0x00, 0x01 },
        "bing.com", RecordTypes.A, RecordClasses.IN
    })]
    [DataRow(new object[]
    {
        new byte[] { 0x04, 0x6D, 0x61, 0x70, 0x73, 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x01, 0x00, 0x01 },
        "maps.bing.com", RecordTypes.A, RecordClasses.IN
    })]
    public void Read(byte[] buffer, string qName, RecordTypes qType, RecordClasses qClass)
    {
        int ptr = 0;

        var question = Question.Read(buffer, ref ptr);
        Equal(question, qName, qType, qClass);
    }

    [DataTestMethod]
    [DataRow(new object[] { "bing.com", RecordTypes.AAAA, RecordClasses.CH })]
    public void Write(string qName, RecordTypes qType, RecordClasses qClass)
    {
        var buffer = new byte[Constants.MaxLabelsTotalLength];
        int ptr = 0;

        var question = new Question()
        {
            name = new DnsName(qName),
            type = qType,
            @class = qClass
        };
        question.Write(buffer, ref ptr);

        Array.Resize(ref buffer, ptr);
        ptr = 0;

        var question2 = Question.Read(buffer, ref ptr);
        Equal(question2, qName, qType, qClass);
    }

    [DataTestMethod]
    [DataRow("bing.com", RecordTypes.A, RecordClasses.IN)]
    public void Equals(string qName, RecordTypes qType, RecordClasses qClass)
    {
        Question Make(string qName, RecordTypes qType, RecordClasses qClass)
        {
            return new()
            {
                name = new DnsName(qName),
                type = qType,
                @class = qClass
            };
        }

        var q1 = Make(qName, qType, qClass);
        var q2 = Make(qName, qType, qClass);

        Assert.AreEqual(q1, q2);
        Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
    }

    [DataTestMethod]
    [DataRow(new object[] { new byte[] { 0x04, 0x6D, 0x61, 0x70, 0x73, 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x01, 0x00, 0x01 } })]
    public void Symmetric(byte[] buffer)
    {
        int ptr = 0;
        var question = Question.Read(buffer, ref ptr);
        var newBuffer = wDNS.Common.Helpers.BufferHelpers.WriteBuffer(question);

        Assert.AreEqual(buffer.Length, newBuffer.Length);
        CollectionAssert.AreEqual(buffer, newBuffer);
    }

    private static void Equal(Question question, string qName, RecordTypes qType, RecordClasses qClass)
    {
        Assert.AreEqual(qName, question.name.Name);
        Assert.AreEqual(qType, question.type);
        Assert.AreEqual(qClass, question.@class);
    }
}
