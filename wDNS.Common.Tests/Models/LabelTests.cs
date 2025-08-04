using Microsoft.VisualStudio.TestTools.UnitTesting;
using wDNS.Common.Models;
using wDNS.Common.Tests.Helpers;

namespace wDNS.Common.Tests.Models;

[TestClass]
public sealed class LabelTests
{
    /*[TestMethod]
    public void ReadPointer_ReturnsValidPointer()
    {
        const int Pointer = 5062;

        var ctx = BufferContextHelpers.Create();
        ctx.WriteUInt16(Pointer);
        ctx.buffer[ctx.pointer - 2] |= 0b1100_0000;

        Label.TryGetPointer(ctx.buffer, 0, out var pointer);
        Assert.AreEqual(Pointer, pointer);
    }*/

    [TestMethod]
    public void Read_SingleSegment_ReturnsValidLabel()
    {
        const string str = "www";
        var ctx = BufferContextHelpers.CreateWithLabel(str);

        var label = new Label();
        label.Read(ctx);

        Assert.AreEqual(1, label.segments!.Length);
        Assert.AreEqual(str.Length, label.segments[0].Length);
        Assert.AreEqual(str, label.segments[0]);
    }

    [TestMethod]
    public void Read_ReturnsValidLabel()
    {
        var ctx = BufferContextHelpers.CreateWithLabel(Constants.Words);

        var label = new Label();
        label.Read(ctx);

        CollectionAssert.AreEqual(Constants.Words, label.segments);
        Assert.AreEqual(Label.Terminator, ctx.CurrentByte);
    }

    [TestMethod]
    public void ReadPointer_ShouldResult_InFullLabel()
    {
        var ctx = BufferContextHelpers.CreateWithLabel();
        var label = new Label()
        {
            segments = [new("www"), new("example"), new("com")]
        };

        label.Write(ctx);
        label.Write(ctx);
    }
}
