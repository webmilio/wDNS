using System.Text;

namespace wDNS.Conversion.Readable;

public interface IReadableStringConverter<T>
{
    public void AppendReadableString(StringBuilder dst, T instance, int spacing);
}
