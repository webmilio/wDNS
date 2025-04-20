using wDNS.Common.Models;

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
        label.Read(ctx);

        Assert.AreEqual(1, label.segments!.Length);
        Assert.AreEqual(str.Length, label.segments[0].length);
        Assert.AreEqual(str, label.segments[0].value);
    }

    [TestMethod]
    public void Read_ShouldResult_InValidLabel()
    {

    }
}
