using wDNS.Common.Models;
using wDNS.Common.Tests.Helpers;

namespace wDNS.Common.Tests.Models;

[TestClass]
public class MessageTests
{
    [TestMethod]
    public void Write_ShouldReturn_NonEmptyBuffer()
    {
        var ctx = BufferContextHelpers.Create();
        var notExpected = new byte[ctx.buffer.Length];

        var write = new Message()
        {
            id = 666,
            answerCount = 4,
            authorityCount = 3,
            RecursionDesired = true,
        };
        write.Write(ctx);

        CollectionAssert.AreNotEqual(notExpected, ctx.buffer);
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

    [TestMethod]
    public void Equality_True_ForSameFields()
    {
        var x = new Message()
        {
            id = 666,
            answerCount = 4,
            authorityCount = 3,
            RecursionDesired = true,
        };
        var y = new Message()
        {
            id = 666,
            answerCount = 4,
            authorityCount = 3,
            RecursionDesired = true,
        };

        Assert.AreEqual(x, y);
        Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
    }

    [TestMethod]
    public void Equality_False_ForDifferentFields()
    {
        var x = new Message()
        {
            id = 666,
            answerCount = 4,
            authorityCount = 3,
            RecursionDesired = true,
        };
        var y = new Message()
        {
            id = 777,
            answerCount = 4,
            authorityCount = 33,
            RecursionDesired = false,
        };

        Assert.AreNotEqual(x, y);
        Assert.AreNotEqual(x.GetHashCode(), y.GetHashCode());
    }
}
