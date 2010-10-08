#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Lib.Hashing.ConsistentHashing
{
    /// <summary>
    /// this is the interface for caches or servers which using the consistent hashing method
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface ICache<TKey, TValue>
    {
        String ItemName { get; }

        TValue GetItem(TKey key);

        void   AddItem(TKey key, TValue value);

        void   RemoveItem(TKey key);

        void   RemoveAllItems();

        IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        Int64  Count();
    }
}
