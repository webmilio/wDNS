namespace wDNS.Common.Models;

public struct Request : IBufferWritable, IBufferReadable
{
    public Message message;
    public Question question;

    public void Read(BufferContext context)
    {
        message.Read(context);
        question.Read(context);
    }

    public readonly void Write(BufferContext context)
    {
        message.Write(context);
        question.Write(context);
    }
}
