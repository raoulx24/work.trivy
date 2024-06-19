using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using TrivyOperator.Dashboard.Infrastructure.Abstractions;

namespace TrivyOperator.Dashboard.Infrastructure.Services;

public class ConcurrentCache<TKey, TValue> : IConcurrentCache<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> dictionary = new();

    public TValue this[TKey key] => dictionary[key];

    public IEnumerable<TKey> Keys => dictionary.Keys;

    public IEnumerable<TValue> Values => dictionary.Values;

    public int Count => dictionary.Count;

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    public bool TryAddValue(TKey key, TValue value)
    {
        return dictionary.TryAdd(key, value);
    }

    public bool TryRemoveValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return dictionary.TryRemove(key, out value);
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    public void Clear()
    {
        dictionary.Clear();
    }
}
