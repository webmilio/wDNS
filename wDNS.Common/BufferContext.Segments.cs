using System.Collections.Generic;
using System;
using wDNS.Common.Models;

namespace wDNS.Common;

public partial class BufferContext
{
    private readonly Dictionary<byte, LabelSegment> _ptrLabels = [];
    private readonly Dictionary<string, byte> _valuePtr = new(StringComparer.OrdinalIgnoreCase);

    public bool TryGetSegment(byte pointer, out LabelSegment segment) => _ptrLabels.TryGetValue(pointer, out segment);
    public bool TryAddSegment(byte pointer, LabelSegment segment) => _ptrLabels.TryAdd(pointer, segment);

    public bool TryAddSegment(LabelSegment segment, byte pointer)
    {
        if (_valuePtr.TryAdd(segment.value, pointer))
        {
            return TryAddSegment(pointer, segment);
        }

        return false;
    }

    public bool TryGetSegmentPointer(string value, out byte pointer)
    {
        return _valuePtr.TryGetValue(value, out pointer);
    }

    public bool TryGetSegment(string value, out LabelSegment segment)
    {
        segment = default;

        if (TryGetSegmentPointer(value, out var pointer))
        {
            return TryGetSegment(pointer, out segment);
        }

        return false;
    }
}
