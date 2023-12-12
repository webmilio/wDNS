using wDNS.Models;

namespace wDNS.Tests.Models;

[TestClass]
public class QuestionTests
{
    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] { 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x01, 0x00, 0x01 },
        "bing.com", QTypes.A, QClasses.IN
    })]
    [DataRow(new object[]
    {
        new byte[] { 0x04, 0x6D, 0x61, 0x70, 0x73, 0x04, 0x62, 0x69, 0x6E, 0x67, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x01, 0x00, 0x01 },
        "maps.bing.com", QTypes.A, QClasses.IN
    })]
    public void Receive(byte[] buffer, string qName, QTypes qType, QClasses qClass)
    {
        int ptr = 0;

        var question = Question.Read(buffer, ref ptr);
        Equal(question, qName, qType, qClass);
    }

    [DataTestMethod]
    [DataRow(new object[] { "bing.com", QTypes.AAAA, QClasses.CH })]
    public void Send(string qName, QTypes qType, QClasses qClass)
    {
        var buffer = new byte[Constants.UdpPacketMaxLength];
        int ptr = 0;

        var question = new Question()
        {
            QName = qName,
            QType = qType,
            QClass = qClass
        };
        question.Write(buffer, ref ptr);

        Array.Resize(ref buffer, ptr);
        ptr = 0;

        var question2 = Question.Read(buffer, ref ptr);
        Equal(question2, qName, qType, qClass);
    }

    private static void Equal(Question question, string qName, QTypes qType, QClasses qClass)
    {
        Assert.AreEqual(qName, question.QName);
        Assert.AreEqual(qType, question.QType);
        Assert.AreEqual(qClass, question.QClass);
    }
}
