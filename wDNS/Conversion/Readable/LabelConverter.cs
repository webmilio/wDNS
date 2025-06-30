using System.Text;
using wDNS.Common.Models;

namespace wDNS.Conversion.Readable;

public interface ILabelConverter : IReadableStringConverter<Label>;

public class LabelConverter : ILabelConverter
{
    private readonly IReadableStringConverter<LabelSegment> _segment;

    public LabelConverter(IReadableStringConverter<LabelSegment> segment)
    {
        _segment = segment;
    }

    public void AppendReadableString(StringBuilder dst, Label instance, int spacing)
    {
        dst.AppendLineSpaced(spacing, $"Segments: {instance.segments.Length}");
        spacing++;

        for (int i = 0; i < instance.segments.Length; i++)
        {
            _segment.AppendReadableString(dst, instance.segments[i], spacing);
        }
    }
}
