using wDNS.Common.Models;
using wDNS.Common.Tests.Helpers;

namespace wDNS.Common.Tests.Models;

[TestClass]
public sealed class LabelTests
{
    [TestMethod]
    public void Read_SingleSegment_ReturnsValidLabel()
    {
        const string str = "www";
        var ctx = BufferContextHelpers.CreateWithLabel(str);

        var label = new Label();
        label.ReadSeq(ctx);

        Assert.AreEqual(1, label.segments!.Length);
        Assert.AreEqual(str.Length, label.segments[0].length);
        Assert.AreEqual(str, label.segments[0].value);
    }

    [TestMethod]
    public void Read_ReturnsValidLabel()
    {
        var ctx = BufferContextHelpers.CreateWithLabel(Constants.Words);

        var label = new Label();
        label.ReadSeq(ctx);

        CollectionAssert.AreEqual(Constants.Words, label.segments!.Select(x => x.value).ToArray());
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
