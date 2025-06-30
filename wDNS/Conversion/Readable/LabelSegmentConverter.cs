using System.Text;
using wDNS.Common.Models;

namespace wDNS.Conversion.Readable;

public interface ILabelSegmentConverter : IReadableStringConverter<LabelSegment>;

public class LabelSegmentConverter : ILabelSegmentConverter
{
    public void AppendReadableString(StringBuilder dst, LabelSegment instance, int spacing)
    {
        dst.AppendLineSpaced(spacing, $"Value: {instance.value}");
    }
}
