#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using sones.Plugins.Index.Interfaces;
using sones.Plugins.Index.Helper;
using System.Threading;
using sones.Plugins.Index.ErrorHandling;

#endregion

namespace sones.Plugins.Index
{
    /// <summary>
    /// This class realize an multivalue index.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class MultipleValueIndex<TKey, TValue> : IMultipleValueIndex<TKey, TValue> where TKey : IComparable
    {

        #region Data

        /// <summary>
        /// The internal index data structure.
        /// </summary>
        private ConcurrentDictionary<TKey, HashSet<TValue>> _Indexer;

        #endregion

        #region Constructors

        /// <summary>
        /// The class constructor.
        /// </summary>
        public MultipleValueIndex()
        {
            _Indexer = new ConcurrentDictionary<TKey, HashSet<TValue>>();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Adds values to the internal data structure.
        /// </summary>
        /// <param name="myKey">The index key.</param>
        /// <param name="myValues">The index values.</param>
        /// <param name="myIndexAddStrategy">The add strategy.</param>
        private void AddValues(TKey myKey, IEnumerable<TValue> myValues, IndexAddStrategy myIndexAddStrategy)
        {
            switch (myIndexAddStrategy)
            {
                case IndexAddStrategy.MERGE:

                    if (!Contains(myKey, myValues))
                    {
                        _Indexer.TryAdd(myKey, new HashSet<TValue>(myValues));
                    }
                    else
                    {
                        foreach (var aValue in myValues)
                        {
                            _Indexer[myKey].Add(aValue);
                        }
                    }

                    break;

                case IndexAddStrategy.UNIQUE:

                    if (Contains(myKey, myValues))
                    {
                        throw new UniqueIndexConstraintException("Index values already exist.");
                    }
                    else
                    {
                        _Indexer.TryAdd(myKey, new HashSet<TValue>(myValues));
                    }

                    break;

                case IndexAddStrategy.REPLACE:

                    _Indexer.TryAdd(myKey, new HashSet<TValue>(myValues));

                    break;
            }
        }

        #endregion

        #region IMultipleValueIndex

        public IEnumerable<TValue> this[TKey myKey]
        {
            get
            {
                return _Indexer[myKey].AsEnumerable();
            }
            set
            {
                _Indexer[myKey] = new HashSet<TValue>(value);
            }
        }

        public void Add(TKey myKey, IEnumerable<TValue> myValues, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            AddValues(myKey, myValues, myIndexAddStrategy);
        }

        public void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            AddValues(myKeyValuesPair.Key, myKeyValuesPair.Value, myIndexAddStrategy);
        }

        public void Add(IDictionary<TKey, IEnumerable<TValue>> myDictionary, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            foreach (var aItem in myDictionary)
            {
                AddValues(aItem.Key, aItem.Value, myIndexAddStrategy);
            }
        }

        public bool Contains(TKey myKey, IEnumerable<TValue> myValues)
        {
            HashSet<TValue> val = null;
            
            if (!_Indexer.TryGetValue(myKey, out val))
            {
                return false;
            }

            foreach (var aValue in val)
            {
                if (!myValues.Contains(aValue))
                {
                    return false;
                }
            }

            return true;

        }

        public IEnumerable<IEnumerable<TValue>> Values()
        {
            return _Indexer.Values.AsEnumerable();
        }

        public bool IsPersistent
        {
            get { return false; }
        }

        public string Name
        {
            get { return "MultiValueIndex"; }
        }

        public long KeyCount()
        {
            return _Indexer.Keys.Count;
        }

        public long ValueCount()
        {
            return _Indexer.Values.Count;
        }

        public IEnumerable<TKey> Keys()
        {
            return _Indexer.Keys.AsEnumerable();
        }       

        public bool ContainsKey(TKey myKey)
        {
            return _Indexer.ContainsKey(myKey);
        }

        public bool ContainsValue(TValue myValue)
        {
            return _Indexer.Any((item) => item.Value.Any((val => val.Equals(myValue))));            
        }

        public bool Contains(TKey myKey, TValue myValue)
        {
            HashSet<TValue> val = null;

            if (_Indexer.TryGetValue(myKey, out val))
            {
                return val.Any(item => item.Equals(myValue));
            }

            return false;
        }

        public bool Remove(TKey myKey)
        {
            HashSet<TValue> val = null;

            return _Indexer.TryRemove(myKey, out val);
        }

        public IEnumerator<KeyValuePair<TKey, IEnumerable<TValue>>> GetEnumerator()
        {            
            return _Indexer.Select(item => new KeyValuePair<TKey, IEnumerable<TValue>>(item.Key, item.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Indexer.GetEnumerator();
        }

        public void ClearIndex()
        {
            _Indexer.Clear();
        }

        #endregion        
    }
}
