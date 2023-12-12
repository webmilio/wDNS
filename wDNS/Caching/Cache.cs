namespace wDNS.Caching;

internal class Cache<K, V>
{
    public record CachedValue(V Value, DateTime Expiry)
    {
        public static List<CachedValue> Make(IList<V> values, Func<int, V, DateTime> expiry)
        {
            var result = new List<CachedValue>(values.Count);

            for (int i = 0; i < values.Count; i++)
            {
                result.Add(new(values[i], expiry(i, values[i])));
            }

            return result;
        }
    }

    private readonly List<K> _keys = [];
    private readonly List<List<V>> _items = [];
    private readonly List<List<DateTime>> _expiries = [];

    private volatile object _lock = new();

    public void Update()
    {
        var now = DateTime.Now;

        for (int i = _items.Count - 1; i >= 0; i--)
        {
            var items = _items[i];

            for (int j = items.Count - 1; j >= 0; j--)
            {
                if (_expiries[i][j] <= now)
                {
                    TryRemoveAt(i, j);
                }
            }
        }
    }

    public bool TryAdd(K key, List<CachedValue> values, out List<CachedValue> result)
    {
        lock (_lock)
        {
            var index = _keys.IndexOf(key);

            if (index > -1)
            {
                result = CachedValue.Make(_items[index], (i, _) => _expiries[index][i]);
                return false;
            }

            var keyIndex = _keys.Count;

            _keys.Add(key);

            List<V> mValues = new();
            List<DateTime> mExpiries = new();

            for (int i = 0; i < values.Count; i++)
            {
                mValues.Add(values[i].Value);
                mExpiries.Add(values[i].Expiry);
            }

            _items.Add(mValues);
            _expiries.Add(mExpiries);

            result = values;
            return true;
        }
    }

    public bool TryRemove(K key)
    {
        var index = _keys.IndexOf(key);
        return TryRemoveAt(index);
    }

    private bool TryRemoveAt(int keyIndex, int valueIndex)
    {
        lock (_lock)
        {
            _items[keyIndex].RemoveAt(valueIndex);
            _expiries[keyIndex].RemoveAt(valueIndex);

            if (_items.Count == 0)
            {
                _items.RemoveAt(keyIndex);
                _keys.RemoveAt(keyIndex);
            }
        }

        return true;
    }

    private bool TryRemoveAt(int keyIndex)
    {
        lock (_lock)
        {
            if (keyIndex >= _keys.Count)
            {
                return false;
            }

            _expiries.RemoveAt(keyIndex);
            _items.RemoveAt(keyIndex);
            _keys.RemoveAt(keyIndex);

            return true;
        }
    }

    public bool TryGetValue(K key, out IList<V> value)
    {
        value = default;

        lock (_lock)
        {
            var index = _keys.IndexOf(key);

            if (index < 0)
            {
                return false;
            }

            value = _items[index];
        }

        return true;
    }
}
