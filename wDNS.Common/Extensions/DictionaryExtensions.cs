namespace wDNS.Common.Extensions;

public static class DictionaryExtensions
{
    public static V GetOrProvide<K, V>(this IDictionary<K, V> dictionary, K key, Func<V> provider) => dictionary.GetOrProvide(key, _ => provider());

    public static V GetOrProvide<K, V>(this IDictionary<K, V> dictionary, K key, Func<K, V> provider)
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }

        dictionary.Add(key, provider(key));
        return dictionary[key];
    }
}
