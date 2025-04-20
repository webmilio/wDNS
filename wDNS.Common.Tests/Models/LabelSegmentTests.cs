using System.Text;
using wDNS.Common.Models;

namespace wDNS.Common.Tests.Models;

[TestClass]
public class LabelSegmentTests
{
    [TestMethod]
    public void Read_ValidBuffer_ReturnsValidSegment()
    {
        const string str = "www";

        var ctx = BufferContextHelpers.CreateWithLabel(str);

        var segment = new LabelSegment();
        segment.Read(ctx);

        Assert.AreEqual(str.Length, segment.length);
        Assert.AreEqual(str, segment.value);
    }
}
