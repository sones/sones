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
#endregion

namespace sones.Plugins.Index
{
    /// <summary>
    /// This class realize a single value index.
    /// </summary>
    /// <typeparam name="TKey">The type of the index key.</typeparam>
    /// <typeparam name="TValue">The type of the index value.</typeparam>
    public class SingleValueIndex<TKey, TValue> : ISingleValueIndex<TKey, TValue>, IPluginable
        where TKey : IComparable
    {
        #region Data

        /// <summary>
        /// The internal index data structure.
        /// </summary>
        private readonly ConcurrentDictionary<TKey, TValue> _Indexer;

        #endregion

        #region Constructors

        /// <summary>
        /// The class constructor.
        /// </summary>
        public SingleValueIndex()
        {
            _Indexer = new ConcurrentDictionary<TKey, TValue>();
        }
        
        #endregion

        #region Private Helper

        /// <summary>
        /// Adds values to the internal data structure.
        /// </summary>
        /// <param name="myKey">The index key.</param>
        /// <param name="myValues">The index value.</param>
        /// <param name="myIndexAddStrategy">The add strategy.</param>
        private void AddValue(TKey myKey, TValue myValue, IndexAddStrategy myIndexAddStrategy)
        {
            switch (myIndexAddStrategy)
            { 
                case IndexAddStrategy.MERGE:

                case IndexAddStrategy.REPLACE:

                    _Indexer.AddOrUpdate(myKey,
                        myValue,
                        (key, oldValue) =>
                        {
                            oldValue = myValue;

                            return oldValue;
                        });

                    break;

                case IndexAddStrategy.UNIQUE:

                    if (_Indexer.ContainsKey(myKey))
                    {
                        throw new UniqueIndexConstraintException(String.Format("Index key {0} already exist.", myKey.ToString()));
                    }
                    else
                    {
                        _Indexer.TryAdd(myKey, myValue);
                    }

                    break;                
            }
        }

        #endregion
        
        
        #region ISingleValueIndex

        public TValue this[TKey myKey]
        {
            get
            {
                if (ContainsKey(myKey))
                {
                    return _Indexer[myKey];
                }
                else
                {
                    return default(TValue);
                }
            }
            set
            {
                AddValue(myKey, value, IndexAddStrategy.REPLACE);
            }
        }

        public void Add(TKey myKey, TValue myValue, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            AddValue(myKey, myValue, myIndexAddStrategy);
        }

        public void Add(KeyValuePair<TKey, TValue> myKeyValuePair, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            AddValue(myKeyValuePair.Key, myKeyValuePair.Value, myIndexAddStrategy);
        }

        public void Add(IDictionary<TKey, TValue> myDictionary, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE)
        {
            foreach (var item in myDictionary)
            {
                AddValue(item.Key, item.Value, myIndexAddStrategy);
            }
        }

        public ISet<TValue> Values()
        {
            return new HashSet<TValue>(_Indexer.Values.AsEnumerable());
        }

        public bool IsPersistent
        {
            get { return false; }
        }

        public string Name
        {
            get { return "sones.singlevalueindex"; }
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
            return new HashSet<TKey>(_Indexer.Keys.AsEnumerable());
        }

        public bool ContainsKey(TKey myKey)
        {
            return _Indexer.ContainsKey(myKey);
        }

        public bool ContainsValue(TValue myValue)
        {
            foreach (var item in _Indexer)
            {
                if (item.Value.Equals(myValue))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(TKey myKey, TValue myValue)
        {
            if (!ContainsKey(myKey))
            {
                return false;
            }

            return _Indexer[myKey].Equals(myValue);
        }

        public bool Remove(TKey myKey)
        {
            TValue val = default(TValue);

            return _Indexer.TryRemove(myKey, out val);
        }

        public void ClearIndex()
        {
            _Indexer.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var retValue = new List<KeyValuePair<TKey, TValue>>();

            foreach (var item in _Indexer)
            {
                retValue.Add(new KeyValuePair<TKey, TValue>(item.Key, item.Value));
            }

            return retValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Indexer.GetEnumerator();
        }

        #endregion

        #region IPluginable Members

        public String PluginName
        {
            get { return "sones.singlevalueindex"; }
        }

        public String PluginShortName
        {
            get { return "singlevalidx"; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get
            {
                return new PluginParameters<Type> 
                { 
                };
            }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<String, Object> myParameters)
        {

            var result = new SingleValueIndex<TKey, TValue>();

            return (IPluginable)result;
        }

        public void Dispose()
        { }

        #endregion

        #region ISonesIndex Members

        public string IndexName
        {
            get { return "singlevalueindex"; }
        }

        #endregion
    }
}
