using System.Text;
using wDNS.Common.Models;

namespace wDNS.Conversion.Readable;

public interface ILabelConverter : IReadableStringConverter<Label>;

public class LabelConverter : ILabelConverter
{
    public void AppendReadableString(StringBuilder dst, Label instance, int spacing)
    {
        dst.AppendLineSpaced(spacing, $"Segments: {instance.segments.Length}");
        spacing++;

        for (int i = 0; i < instance.segments.Length; i++)
        {
            dst.AppendLineSpaced(spacing, instance.segments[i]);
        }
    }
}
