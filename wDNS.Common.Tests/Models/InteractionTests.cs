using Microsoft.VisualStudio.TestTools.UnitTesting;
using wDNS.Common.Models;
using wDNS.Common.Tests.Helpers;

namespace wDNS.Common.Tests.Models;

[TestClass]
public class InteractionTests
{
    [TestMethod]
    public void Write_ShouldReturn_NonEmptyBuffer()
    {
        var ctx = BufferContextHelpers.Create();
        var emptyCtx = new byte[ctx.buffer.Length];

        var write = new Request()
        {
            message = new Message()
            {
                id = 666
            },
            question = new Question()
            {
                name = LabelHelpers.CreateFromConstants(),
                @class = RecordClasses.IN
            }
        };
        write.Write(ctx);

        CollectionAssert.AreNotEqual(emptyCtx, ctx.buffer);
    }

    [TestMethod]
    public void WriteRead_ShouldReturn_ValidRequest()
    {
        var ctx = BufferContextHelpers.Create();

        var write = new Request()
        {
            message = new Message()
            {
                id = 666
            },
            question = new Question()
            {
                name = LabelHelpers.CreateFromConstants(),
                @class = RecordClasses.IN
            }
        };
        write.Write(ctx);
        ctx.ResetPointer();

        var read = new Request();
        read.ReadSeq(ctx);

        Assert.AreEqual(write, read);
    }
}
