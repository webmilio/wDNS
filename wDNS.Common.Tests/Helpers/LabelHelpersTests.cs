using Microsoft.VisualStudio.TestTools.UnitTesting;
using H = wDNS.Common.Tests.Helpers.LabelHelpers;

namespace wDNS.Common.Tests.Helpers;

[TestClass]
public class LabelHelpersTests
{
    [TestMethod]
    public void Create_ShouldReturn_CorrectSegments()
    {
        var label = H.Create(Constants.Words);

        Assert.AreEqual(Constants.Words.Length, label.segments.Length);
        CollectionAssert.AreEqual(Constants.Words, label.segments);
    }
}
