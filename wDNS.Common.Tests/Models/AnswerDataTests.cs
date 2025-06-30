using Microsoft.VisualStudio.TestTools.UnitTesting;
using wDNS.Common.Models;
using wDNS.Common.Tests.Helpers;

namespace wDNS.Common.Tests.Models;

[TestClass]
public class AnswerDataTests
{
    [TestMethod]
    public void Write_ShouldReturn_NonEmptyBuffer()
    {
        var rdata = new byte[] { 50, 10, 15, 30 };

        var ctx = BufferContextHelpers.Create();
        var notExpected = new byte[ctx.buffer.Length];

        var write = new Answer()
        {
            name =
            {
                segments = []
            },
            type = RecordTypes.CNAME,
            @class = RecordClasses.CH,

            ttl = 57,
            rdlength = (ushort) rdata.Length,
            rdata = rdata
        };
        write.Write(ctx);

        CollectionAssert.AreNotEqual(notExpected, ctx.buffer);
    }

    [TestMethod]
    public void WriteRead_ShouldReturn_ValidAnswerData()
    {
        var rdata = new byte[] { 50, 10, 15, 30 };
        var ctx = BufferContextHelpers.Create();

        var write = new Answer()
        {
            name =
            {
                segments = []
            },
            type = RecordTypes.CNAME,
            @class = RecordClasses.CH,

            ttl = 57,

            rdlength = (ushort)rdata.Length,
            rdata = rdata
        };
        write.Write(ctx);
        ctx.ResetPointer();

        var read = new Answer();
        read.ReadSeq(ctx);

        Assert.AreEqual(write, read); // I don't particularly like this since it means we have to update the .Equals()...
    }
}
