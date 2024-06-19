using System.Diagnostics.CodeAnalysis;

namespace TrivyOperator.Dashboard.Infrastructure.Abstractions;

public interface IConcurrentCache<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
{
    bool TryAddValue(TKey key, TValue value);
    bool TryRemoveValue(TKey key, [MaybeNullWhen(false)] out TValue value);
    void Clear();
}
