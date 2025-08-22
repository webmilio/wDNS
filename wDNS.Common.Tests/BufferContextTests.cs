using wDNS.Common.Models;
using wDNS.Common.Tests.Helpers;

namespace wDNS.Common.Tests;

[TestClass]
public class BufferContextTests
{
    [TestMethod]
    public void ReadWriteByte_ShouldReturn_TheSameValue()
    {
        var ctx = BufferContextHelpers.Create();
        
        byte toWrite = 51;
        ctx.WriteByte(toWrite);

        ctx.ResetPointer();
        var read = ctx.ReadByte();

        Assert.AreEqual(toWrite, read);
    }

    [TestMethod]
    public void ReadWriteUInt16_ShouldReturn_TheSameValue()
    {
        var ctx = BufferContextHelpers.Create();

        ushort toWrite = 0b11110000_10010000;
        ctx.WriteUInt16(toWrite);

        ctx.ResetPointer();
        var read = ctx.ReadUInt16();

        Assert.AreEqual(toWrite, read);
    }

    [TestMethod]
    public void ReadWriteUInt32_ShouldReturn_TheSameValue()
    {
        var ctx = BufferContextHelpers.Create();

        uint toWrite = 0b11110000_10010000_00000000_11111111;
        ctx.WriteUInt32(toWrite);

        ctx.ResetPointer();
        var read = ctx.ReadUInt32();

        Assert.AreEqual(toWrite, read);
    }

    [TestMethod]
    public void ReadWriteByte_MultipleTimes_ShouldReturn_TheSameValues()
    {
        var ctx = BufferContextHelpers.Create();
        var values = new byte[] { 44, 222, 255, byte.MaxValue, 0 };

        for (int i = 0; i < values.Length; i++)
        {
            ctx.WriteByte(values[i]);
        }

        ctx.ResetPointer();
        var read = new byte[values.Length];

        for (int i = 0; i < read.Length; i++)
        {
            read[i] = ctx.ReadByte();
        }

        CollectionAssert.AreEqual(values, read);
    }

    [TestMethod]
    public void ReadWriteCastedEnums_ShouldReturn_TheSameValue()
    {
        var ctx = BufferContextHelpers.Create();

        var @class = RecordClasses.ANY;
        var type = RecordTypes.AVC;

        ctx.WriteUInt16((ushort)@class);
        ctx.WriteUInt16((ushort)type);

        ctx.ResetPointer();

        var readClass = (RecordClasses) ctx.ReadUInt16();
        var readType = (RecordTypes) ctx.ReadUInt16();

        Assert.AreEqual(@class, readClass);
        Assert.AreEqual(type, readType);
    }
}
