using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index;
using ISonesIndex;
using System.Collections;

namespace SonesIndices
{
    public class BTreeIndex<TKey, TValue> : IRangeIndex<TKey, TValue>, IMultipleValueIndex<TKey, TValue>
        where TKey : IComparable
    {
        #region IIndex Members

        #region Properties

        public bool IsPersistent
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Add

        public void Add(TKey myKey, TValue myValue, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, TValue> myKeyValuePair, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        public void Add(Dictionary<TKey, TValue> myDictionary, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            throw new NotImplementedException();
        }

        TValue IIndex<TKey, TValue>.this[TKey myKey]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Contains

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

        #endregion

        #region Remove

        public bool Remove(TKey myKey)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Clear

        public void Clear()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Keys

        public IEnumerable<TKey> Keys
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Values

        public IEnumerable<TValue> GetValues()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Counts

        public long KeyCount
        {
            get { throw new NotImplementedException(); }
        }

        public long GetValueCount()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region IRangeIndex Members

        public IEnumerable<TValue> GreaterThan(TKey myKey, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipleValueIndex Members

        #region Values

        IEnumerable<IEnumerable<TValue>> IMultipleValueIndex<TKey, TValue>.GetValues()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> this[TKey myKey, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region this

        IEnumerable<TValue> IMultipleValueIndex<TKey, TValue>.this[TKey myKey]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #endregion

        #region IEnumerable<TValue> Members

        public IEnumerator<TValue> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #region Add

        public void Add(TKey myKey, IEnumerable<TValue> myValues, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            throw new NotImplementedException();
        }

        public void Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Contains

        public bool Contains(TKey myKey, IEnumerable<TValue> myValues)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
