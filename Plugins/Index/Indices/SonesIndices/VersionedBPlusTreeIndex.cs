using System;
using System.Collections;
using System.Collections.Generic;
using sones.Plugins.Index.Helper;
using sones.Plugins.Index.Interfaces;

namespace SonesIndices
{
    public class VersionedBPlusTreeIndex<TKey, TValue, TVersion> :
        IMultipleValueRangeVersionedIndex<TKey, TValue, TVersion>
        where TKey : IComparable
        where TVersion : IComparable
    {
        #region IMultipleValueRangeVersionedIndex<TKey,TValue,TVersion> Members

        public IEnumerable<IEnumerable<TValue>> GreaterThan(TKey myKey, TVersion myVersion, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<TValue>> LowerThan(TKey myKey, TVersion myVersion, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<TValue>> InRange(TKey myFromKey, TKey myToKey, TVersion myVersion,
                                                        bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<TValue>> GreaterThan(TKey myKey, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<TValue>> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<TValue>> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true,
                                                        bool myOrEqualToKey = true)
        {
            throw new NotImplementedException();
        }

        public void Add(TKey myKey, IEnumerable<TValue> myValues,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            throw new NotImplementedException();
        }

        public void Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> this[TKey myKey]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool Contains(TKey myKey, IEnumerable<TValue> myValues)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<TValue>> Values()
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

        public IEnumerator<KeyValuePair<TKey, IEnumerable<TValue>>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Add(TKey myKey, IEnumerable<TValue> myValue, TVersion myVersion,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuePair, TVersion myVersion,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        public void Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary, TVersion myVersion,
                        IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> this[TKey myKey, TVersion myVersion]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IEnumerable<IEnumerable<TValue>> Values(TVersion myVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TKey> Keys(TVersion myVersion)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey myKey, TVersion myVersion)
        {
            throw new NotImplementedException();
        }

        public bool ContainsValue(TValue myValue, TVersion myVersion)
        {
            throw new NotImplementedException();
        }

        public bool Contains(TKey myKey, TValue myValue, TVersion myVersion)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey myKey, TVersion myVersion)
        {
            throw new NotImplementedException();
        }

        public long HistoryCount()
        {
            throw new NotImplementedException();
        }

        public long VersionCount(TKey myKey)
        {
            throw new NotImplementedException();
        }

        public void ClearHistory()
        {
            throw new NotImplementedException();
        }

        public void ClearHistory(TKey myKey)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}