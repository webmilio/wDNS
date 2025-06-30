using wDNS.Common.Models;
using wDNS.Common.Tests.Helpers;

namespace wDNS.Common.Tests.Models;

[TestClass]
public class LabelSegmentTests
{
    [TestMethod]
    public void Read_ValidBuffer_ReturnsValidSegment()
    {
        var str = Constants.Words[0];
        var ctx = BufferContextHelpers.CreateWithLabel(str);

        var segment = new LabelSegment();
        segment.ReadSeq(ctx);

        Assert.AreEqual(str.Length, segment.length);
        Assert.AreEqual(str, segment.value);
    }

    [TestMethod]
    public void Write_SameSegment_CreatesPointer()
    {
        const string str = "www";
        var ctx = BufferContextHelpers.Create();

        ctx.WriteByte(byte.MaxValue);
        var x = new LabelSegment(str);
        var y = x;

        x.Write(ctx);
        y.Write(ctx);

        Assert.IsTrue(ctx.TryGetSegmentPointer(str, out var pointer));
        Assert.AreNotEqual(byte.MaxValue, pointer);
    }

    [TestMethod]
    public void Read_PointerSegment_ReturnsValidSegment()
    {
        const string str = "www";
        var ctx = BufferContextHelpers.Create();

        var x = new LabelSegment(str);
        var y = x;

        var label = new Label()
        {
            segments = [x, y]
        };
        label.Write(ctx);
    }

    [TestMethod]
    public void Equality_True_ForSameFields()
    {
        var x = new LabelSegment("www");
        var y = new LabelSegment("www");

        Assert.AreEqual(x, y);
        Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
    }

    [TestMethod]
    public void Equality_False_ForDifferentFields()
    {
        var x = new LabelSegment("www");
        var y = new LabelSegment("xxx");

        Assert.AreNotEqual(x, y);
        Assert.AreNotEqual(x.GetHashCode(), y.GetHashCode());
    }
}
