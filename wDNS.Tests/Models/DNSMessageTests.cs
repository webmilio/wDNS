using wDNS.Common;
using wDNS.Common.Models;

namespace wDNS.Tests.Common;

[TestClass]
public class DNSMessageTests
{
    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] { 0x69, 0x34, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
        MessageFlags.Query_Query | MessageFlags.Opcode_Standard | MessageFlags.Authoritative_NonAuthoritative | MessageFlags.Truncation_Entire | MessageFlags.RecursionDesired_Desired | MessageFlags.RecursionAvailable_Unsupported | MessageFlags.ResponseCode_NoError,
        26932, 1, 0, 0, 0
    })]
    public void Receive(byte[] buffer, MessageFlags flags, int identification, int questionCount, int answerCount, int authorityCount, int additionalCount)
    {
        int ptr = 0;
        var message = DnsMessage.Read(buffer, ref ptr);

        Equal(message, flags, identification, questionCount, answerCount, authorityCount, additionalCount);
    }

    [DataTestMethod]
    [DataRow(new object[]
    {
        MessageFlags.Query_Query | MessageFlags.Opcode_ServerStatus | MessageFlags.Authoritative_NonAuthoritative | MessageFlags.Truncation_Entire | MessageFlags.RecursionDesired_Desired | MessageFlags.RecursionAvailable_Unsupported | MessageFlags.ResponseCode_NoError,
        420, 1, 2, 3, 4
    })]
    public void Send(MessageFlags flags, int identification, int questionCount, int answerCount, int authorityCount, int additionalCount)
    {
        var buffer = new byte[Constants.MaxLabelsTotalLength];
        int ptr = 0;

        var message = new DnsMessage()
        {
            identification = (ushort)identification,
            flags = flags,

            questionCount = (ushort)questionCount,
            answerCount = (ushort)answerCount,
            authorityCount = (ushort)authorityCount,
            additionalCount = (ushort)additionalCount
        };

        message.Write(buffer, ref ptr);
        Array.Resize(ref buffer, ptr);

        ptr = 0;
        var message2 = DnsMessage.Read(buffer, ref ptr);

        Equal(message2, flags, identification, questionCount, answerCount, authorityCount, additionalCount);
    }

    private static void Equal(DnsMessage message, MessageFlags flags, int identification, int questionCount, int answerCount, int authorityCount, int additionalCount)
    {
        Assert.AreEqual(identification, message.identification);
        Assert.AreEqual((ushort)flags, (ushort)message.flags);
        Assert.AreEqual(questionCount, message.questionCount);
        Assert.AreEqual(answerCount, message.answerCount);
        Assert.AreEqual(authorityCount, message.authorityCount);
        Assert.AreEqual(additionalCount, message.additionalCount);
    }
}