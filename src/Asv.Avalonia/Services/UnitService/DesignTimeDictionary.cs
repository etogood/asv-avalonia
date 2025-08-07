using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Asv.Avalonia;

public class DesignTimeDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
{
    private readonly KeyValuePair<TKey, TValue> _value;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        yield return _value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => 1;

    public bool ContainsKey(TKey key) => true;

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        value = _value.Value;
        return true;
    }

    public TValue this[TKey key] => _value.Value;

    public IEnumerable<TKey> Keys => [_value.Key];
    public IEnumerable<TValue> Values => [_value.Value];

    public DesignTimeDictionary(KeyValuePair<TKey, TValue> value)
    {
        DesignTime.ThrowIfNotDesignMode();

        _value = value;
    }
}
