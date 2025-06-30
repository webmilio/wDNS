using System.Text;
using wDNS.Common.Models;

namespace wDNS.Conversion.Readable;

public interface IQuestionConverter : IReadableStringConverter<Question>;

public class QuestionConverter : IQuestionConverter
{
    private readonly IReadableStringConverter<Label> _label;

    public QuestionConverter(IReadableStringConverter<Label> label)
    {
        _label = label;
    }

    public void AppendReadableString(StringBuilder dst, Question instance, int spacing)
    {
        dst.AppendLine(ConverterHelpers.SpaceString(spacing, $"Class: {instance.@class}, Types: {instance.types}"));
        _label.AppendReadableString(dst, instance.name, ++spacing);
    }
}
