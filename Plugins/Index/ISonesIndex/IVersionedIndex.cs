using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index;

namespace sones.Plugins.Index
{
    /// <summary>
    /// Defines methods for versioned indices
    /// </summary>
    /// <typeparam name="TKey">The key of the index (which must implement IComparable)</typeparam>
    /// <typeparam name="TValue">The value associated to the key</typeparam>
    /// <typeparam name="TVersionComparer">A comparer which decides the order of revisions</typeparam>
    public interface IVersionedIndex<TKey, TValue, TVersionComparer> : IIndex<TKey, TValue>
        where TKey : IComparable
    {

    }
}
