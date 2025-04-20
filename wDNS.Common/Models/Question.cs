namespace wDNS.Common.Models;

public struct Question : IBufferWritable, IBufferReadable
{
    public Label name;
    public RecordTypes types;
    public RecordClasses @class;

    public void Read(BufferContext context)
    {
        name.Read(context);

        types = (RecordTypes)context.ReadUInt16();
        @class = (RecordClasses)context.ReadUInt16();
    }

    public readonly void Write(BufferContext context)
    {
        name.Write(context);

        context.WriteUInt16((ushort)types);
        context.WriteUInt16((ushort)@class);
    }
}
