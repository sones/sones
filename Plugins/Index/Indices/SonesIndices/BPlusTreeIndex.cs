using System;
using System.Collections;
using System.Collections.Generic;
using sones.Plugins.Index.Helper;
using sones.Plugins.Index.Interfaces;

namespace sones.Plugins.Index
{
    public class BPlusTreeIndex<TKey, TValue> : IMultipleValueRangeIndex<TKey, TValue>
        where TKey : IComparable
    {
        #region IMultipleValueRangeIndex<TKey,TValue> Members

        public IEnumerable<ISet<TValue>> GreaterThan(TKey myKey, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISet<TValue>> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISet<TValue>> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true,
                                                        bool myOrEqualToKey = true)
        {
            throw new NotImplementedException();
        }

        public void Add(TKey myKey, ISet<TValue> myValues,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, ISet<TValue>> myKeyValuesPair,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            throw new NotImplementedException();
        }

        public void Add(IDictionary<TKey, ISet<TValue>> myDictionary,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            throw new NotImplementedException();
        }

        public ISet<TValue> this[TKey myKey]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool Contains(TKey myKey, ISet<TValue> myValues)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISet<TValue>> Values()
        {
            throw new NotImplementedException();
        }

        public bool IsPersistent
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public long KeyCount()
        {
            throw new NotImplementedException();
        }

        public long ValueCount()
        {
            throw new NotImplementedException();
        }

        public ISet<TKey> Keys()
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey myKey)
        {
            throw new NotImplementedException();
        }

        public bool ContainsValue(TValue myValue)
        {
            throw new NotImplementedException();
        }

        public bool Contains(TKey myKey, TValue myValue)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey myKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, ISet<TValue>>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void ClearIndex()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}