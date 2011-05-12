/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

#region Usings

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using sones.Plugins.Index.ErrorHandling;
using sones.Plugins.Index.Helper;
using sones.Plugins.Index.Interfaces;
using sones.Library.VersionedPluginManager;
using sones.Library.Settings;

#endregion

namespace sones.Plugins.Index
{
    /// <summary>
    /// This class realize an multivalue index.
    /// </summary>
    /// <typeparam name="TKey">The type of the index key.</typeparam>
    /// <typeparam name="TValue">The type of the index values.</typeparam>
    public class MultipleValueIndex<TKey, TValue> : IMultipleValueIndex<TKey, TValue> , IPluginable
        where TKey : IComparable
    {

        #region Data

        /// <summary>
        /// The internal index data structure.
        /// </summary>
        private readonly ConcurrentDictionary<TKey, HashSet<TValue>> _Indexer;

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
        private void AddValues(TKey myKey, ISet<TValue> myValues, IndexAddStrategy myIndexAddStrategy)
        {
            switch (myIndexAddStrategy)
            {
                case IndexAddStrategy.MERGE:
                    
                    _Indexer.AddOrUpdate(myKey,
                        new HashSet<TValue>(myValues),
                        (key, oldValue) =>
                        {
                            lock (oldValue)
                            {
                                oldValue.UnionWith(myValues);

                                return oldValue;
                            }
                        });

                    break;

                case IndexAddStrategy.UNIQUE:

                    if (_Indexer.ContainsKey(myKey))
                    {
                        throw new UniqueIndexConstraintException(String.Format("Index key {0} already exist.", myKey.ToString()));
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

        public ISet<TValue> this[TKey myKey]
        {
            get
            {
                if (ContainsKey(myKey))
                {
                    return (ISet<TValue>)_Indexer[myKey].AsEnumerable();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                AddValues(myKey, value, IndexAddStrategy.REPLACE);
            }
        }

        public void Add(TKey myKey, ISet<TValue> myValues, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            AddValues(myKey, myValues, myIndexAddStrategy);
        }

        public void Add(KeyValuePair<TKey, ISet<TValue>> myKeyValuesPair, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            AddValues(myKeyValuesPair.Key, myKeyValuesPair.Value, myIndexAddStrategy);
        }

        public void Add(IDictionary<TKey, ISet<TValue>> myDictionary, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE)
        {
            foreach (var aItem in myDictionary)
            {
                AddValues(aItem.Key, aItem.Value, myIndexAddStrategy);
            }
        }

        public bool Contains(TKey myKey, ISet<TValue> myValues)
        {
            HashSet<TValue> val = null;
            
            if (!_Indexer.TryGetValue(myKey, out val))
            {
                return false;
            }

            if (val != null)
            {
                lock (val)
                {
                    foreach (var aValue in val)
                    {
                        if (!myValues.Contains(aValue))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;

        }

        public IEnumerable<ISet<TValue>> Values()
        {
            return _Indexer.Values.AsEnumerable();
        }

        public bool IsPersistent
        {
            get { return false; }
        }

        public string Name
        {
            get { return "sones.multiplevalueindex"; }
        }

        public long KeyCount()
        {
            return _Indexer.Keys.Count;
        }

        public long ValueCount()
        {
            return _Indexer.Values.Count;
        }

        public ISet<TKey> Keys()
        {            
            return new HashSet<TKey>(_Indexer.Keys);
        }       

        public bool ContainsKey(TKey myKey)
        {
            return _Indexer.ContainsKey(myKey);
        }

        public bool ContainsValue(TValue myValue)
        {
            foreach (var item in _Indexer)
            {
                lock (item.Value)
                {
                    if (item.Value.Any(val => val.Equals(myValue)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Contains(TKey myKey, TValue myValue)
        {
            HashSet<TValue> val = null;

            if (_Indexer.TryGetValue(myKey, out val))
            {
                lock (val)
                {
                    return val.Any(item => item.Equals(myValue));
                }
            }

            return false;
        }

        public bool Remove(TKey myKey)
        {
            HashSet<TValue> val = null;

            return _Indexer.TryRemove(myKey, out val);
        }

        public IEnumerator<KeyValuePair<TKey, ISet<TValue>>> GetEnumerator()
        {
            var retValue = new List<KeyValuePair<TKey, ISet<TValue>>>();
            
            foreach (var item in _Indexer)
            {
                lock (item.Value)
                {
                    retValue.Add(new KeyValuePair<TKey, ISet<TValue>>(item.Key, item.Value));
                }
            }

            return retValue.GetEnumerator();
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

        #region IPluginable Members

        public String PluginName
        {
            get { return "sones.multiplevalueindex"; }
        }

        public Dictionary<String, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string, Type> 
                { 
                };
            }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<String, Object> myParameters)
        {

            var result = new MultipleValueIndex<TKey, TValue>();

            return (IPluginable)result;
        }

        #endregion

        #region ISonesIndex Members

        public string IndexName
        {
            get { return "multiplevalueindex"; }
        }

        #endregion
    }
}
