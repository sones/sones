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

#endregion

namespace sones.Plugin.Index
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
        private ConcurrentDictionary<TKey, ConcurrentBag<TValue>> _Indexer;

        #endregion

        #region Constructors

        /// <summary>
        /// The class constructor.
        /// </summary>
        public MultipleValueIndex()
        {
            _Indexer = new ConcurrentDictionary<TKey, ConcurrentBag<TValue>>();
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
                        _Indexer.TryAdd(myKey, new ConcurrentBag<TValue>(myValues));
                    }
                    else
                    {
                        ConcurrentBag<TValue> val = null;

                        _Indexer.TryGetValue(myKey, out val);

                        Parallel.ForEach(myValues, (item) =>
                        {
                            val.Add(item);
                        });
                    }

                    break;

                case IndexAddStrategy.UNIQUE:

                    if (Contains(myKey, myValues))
                    {
                        throw new ArgumentException("Index values already exist.");
                    }
                    else
                    {
                        _Indexer.TryAdd(myKey, new ConcurrentBag<TValue>(myValues));
                    }

                    break;

                case IndexAddStrategy.REPLACE:

                    _Indexer.AddOrUpdate(myKey, new ConcurrentBag<TValue>(myValues), null);

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
                _Indexer[myKey] = new ConcurrentBag<TValue>(value);
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
            Parallel.ForEach(myDictionary, (item) =>
            {
                AddValues(item.Key, item.Value, myIndexAddStrategy);
            });            
        }

        public bool Contains(TKey myKey, IEnumerable<TValue> myValues)
        {
            if (!_Indexer.ContainsKey(myKey))
            {
                return false;
            }

            return _Indexer.AsParallel().Any(item => item.Value.Equals(myValues));
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
            get { return "MultiValue"; }
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
            return _Indexer.AsParallel().Any((item) => item.Value.Any((val => val.Equals(myValue))));            
        }

        public bool Contains(TKey myKey, TValue myValue)
        {
            ConcurrentBag<TValue> val = null;

            if (_Indexer.TryGetValue(myKey, out val))
            {
                return val.AsParallel().Any(item => item.Equals(myValue));
            }

            return false;
        }

        public bool Remove(TKey myKey)
        {            
            ConcurrentBag<TValue> val = null;

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
