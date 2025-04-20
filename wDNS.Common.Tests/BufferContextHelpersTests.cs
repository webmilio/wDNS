using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using wDNS.Common.Models;

namespace wDNS.Common.Tests;

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
        var segBuffer = new byte[str.Length];

        Array.Copy(ctx.buffer, 1, segBuffer, 0, str.Length);

        CollectionAssert.AreEqual(expected, segBuffer);
    }
}
