using System;
using System.Collections;
using System.Collections.Generic;
using sones.Plugins.Index.Helper;
using sones.Plugins.Index.Interfaces;

namespace sones.Plugins.Index
{
    public class BTreeIndex<TKey, TValue> : ISingleValueIndex<TKey, TValue>
        where TKey : IComparable
    {
        #region ISingleValueIndex<TKey,TValue> Members

        public void Add(TKey myKey, TValue myValue, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, TValue> myKeyValuePair,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        public void Add(Dictionary<TKey, TValue> myDictionary,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        public TValue this[TKey myKey]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IEnumerable<TValue> Values()
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

        public IEnumerable<TKey> Keys()
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

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
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