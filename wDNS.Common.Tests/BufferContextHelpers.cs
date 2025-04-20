using System;
using System.Text;
using wDNS.Common.Models;

namespace wDNS.Common.Tests;

public class BufferContextHelpers
{
    public static BufferContext Create() => new(new byte[wDNS.Common.Constants.MaxUdpSize]);

    public static BufferContext CreateWithLabel(params string[] words)
    {
        var buffer = new List<byte>();

        for (int i = 0; i < words.Length; i++)
        {
            buffer.Add((byte) words[i].Length);
            buffer.AddRange(Encoding.ASCII.GetBytes(words[i]));
        }

        buffer.Add(Label.Terminator);
        return new BufferContext([.. buffer]);
    }
}
