using System;

namespace sones.Plugins.Index.Interfaces
{
    /// <summary>
    /// Default interface for range indices.
    /// </summary>
    /// <typeparam name="TKey">The type of the comparable index key</typeparam>
    /// <typeparam name="TValue">The type of the index value</typeparam>
    public interface IRangeIndex<TKey, TValue>
        : IIndex<TKey, TValue>
        where TKey : IComparable
    {
    }
}