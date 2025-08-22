using System.Text;
using wDNS.Common.Models;

namespace wDNS.Common.Tests.Helpers;

[TestClass]
public class BufferContextHelpersTests
{
    [TestMethod]
    public void Create_Returns_Non0LengthBuffer()
    {
        var ctx = BufferContextHelpers.Create();
        Assert.AreNotEqual(0, ctx.buffer.Length);
    }

    [TestMethod]
    public void CreateWithLabel_Returns_TerminatedBuffer()
    {
        var ctx = BufferContextHelpers.CreateWithLabel("www");
        Assert.AreEqual(Label.Terminator, ctx.buffer[^1]);
    }

    [TestMethod]
    public void CreateWithLabel_Returns_CorrectLength()
    {
        const string str = "www";
        var ctx = BufferContextHelpers.CreateWithLabel(str);

        Assert.AreEqual(str.Length, ctx.buffer[0]);
    }

    [TestMethod]
    public void CreateWithLabel_Returns_CorrectCharacters()
    {
        const char @char = 'w';
        var str = new string(@char, 3);
        var expected = Encoding.ASCII.GetBytes(str);

        var ctx = BufferContextHelpers.CreateWithLabel(str);
        var segBuffer = new byte[str.Length + 2];

        segBuffer[0] = (byte) str.Length;
        segBuffer[^1] = Label.Terminator;
        Array.Copy(expected, 0, segBuffer, 1, expected.Length);

        CollectionAssert.IsSubsetOf(segBuffer, ctx.buffer);
    }
}
