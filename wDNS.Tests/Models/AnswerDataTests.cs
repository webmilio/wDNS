using wDNS.Common;
using wDNS.Common.Helpers;
using wDNS.Common.Models;

namespace wDNS.Tests.Models;

[TestClass]
public class AnswerDataTests
{
    [DataTestMethod]
    [DataRow(new object[]
    {
        new byte[] { 0x00, 0x04, 0xC0, 0x46, 0x0C, 0x63 },
        QTypes.A, 4,
        new byte[] { 0xC0, 0x46, 0x0C, 0x63 }
    })]
    public void Read(byte[] buffer, QTypes qType, int length, byte[] data)
    {
        int ptr = 0;
        var answerData = AnswerData.Read(qType, buffer, ref ptr);

        Assert.AreEqual(length, answerData.Length);
        CollectionAssert.AreEqual(data, answerData.Data);
    }

    [DataTestMethod]
    [DataRow(new object[]
    {
        QTypes.A,
        new byte[] { 0xC0, 0x46, 0x0C, 0x63 }
    })]
    public void Write(QTypes qType, byte[] data)
    {
        var answerData = new AnswerData(qType, (ushort) data.Length, data, null);
        var buffer = wDNS.Common.Helpers.BufferHelpers.WriteBuffer(answerData);

        Read(buffer, qType, data.Length, data);
    }
}
