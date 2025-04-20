using Microsoft.VisualStudio.TestTools.UnitTesting;
using wDNS.Common.Models;

namespace wDNS.Common.Tests.Models;

[TestClass]
public class MessageTests
{
    [TestMethod]
    public void Write_ShouldReturn_NonEmptyBuffer()
    {
        var context = BufferContextHelpers.Create();
        var notExpected = new byte[context.buffer.Length];

        var write = new Message()
        {
            id = 666,
            answerCount = 4,
            authorityCount = 3,
            RecursionDesired = true,
        };
        write.Write(context);

        CollectionAssert.AreNotEqual(notExpected, context.buffer);
    }

    [TestMethod]
    public void WriteRead_ShouldReturn_ValidMessage()
    {
        var context = BufferContextHelpers.Create();

        var write = new Message()
        {
            id = 666,
            answerCount = 4,
            authorityCount = 3,
            RecursionDesired = true,
        };
        write.Write(context);

        context.ResetPointer();

        var read = new Message();
        read.Read(context);

        Assert.AreEqual(write, read);
    }
}
