/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using sones.Lib.DataStructures;
using sones.Lib.NewFastSerializer;
using sones.GraphFS.Session;
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.WeakReference;

namespace sones.GraphFS.Objects
{

    public abstract class AVersionedIndexObject<TKey, TValue> : AFSObject
        where TKey : IComparable
    {

        #region Data

        protected IVersionedIndexObject<TKey, TValue> _IVersionedIndexObject;

        #endregion

        #region Constructors

        public AVersionedIndexObject()
        {
            _IVersionedIndexObject = new VersionedHashIndexObject<TKey, TValue>();
        }

        public AVersionedIndexObject(IVersionedIndexObject<TKey, TValue> myIVersionedIndexObject)
        {
            _IVersionedIndexObject = myIVersionedIndexObject;
        }

        #endregion


        #region IGraphFSReference

        /// <summary>
        /// IGraphFSReference
        /// </summary>
        public new WeakReference<IGraphFS> IGraphFSReference
        {

            get
            {
                return ((AFSObject)_IVersionedIndexObject).IGraphFSReference;
            }

            set
            {
                _IGraphFSReference = value;
                ((AFSObject)_IVersionedIndexObject).IGraphFSReference = value;
            }

        }

        #endregion

        #region IGraphFSSessionReference

        /// <summary>
        /// FileSystemReference
        /// </summary>
        public new WeakReference<IGraphFSSession> IGraphFSSessionReference
        {

            get
            {
                return ((AFSObject)_IVersionedIndexObject).IGraphFSSessionReference;
            }

            set
            {
                _IGraphFSSessionReference = value;
                ((AFSObject)_IVersionedIndexObject).IGraphFSSessionReference = value;
            }

        }

        #endregion


        #region (protected) IIndexObject<TKey,TValue> Members

        protected void Add(TKey myKey, TValue myValue)
        {
            _IVersionedIndexObject.Add(myKey, myValue);
        }

        protected void Add(TKey myKey, IEnumerable<TValue> myValues)
        {
            _IVersionedIndexObject.Add(myKey, myValues);
        }

        protected void Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            _IVersionedIndexObject.Add(myKeyValuePair);
        }

        protected void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
        {
            _IVersionedIndexObject.Add(myKeyValuesPair);
        }

        protected void Add(Dictionary<TKey, TValue> myDictionary)
        {
            _IVersionedIndexObject.Add(myDictionary);
        }

        protected void Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary)
        {
            _IVersionedIndexObject.Add(myDictionary);
        }



        protected void Set(TKey myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
        {
            _IVersionedIndexObject.Set(myKey, myValue, myIndexSetStrategy);
        }

        protected void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
        {
            _IVersionedIndexObject.Set(myKey, myValues, myIndexSetStrategy);
        }

        protected void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
        {
            _IVersionedIndexObject.Set(myKeyValuePair, myIndexSetStrategy);
        }

        protected void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
        {
            _IVersionedIndexObject.Set(myKeyValuesPair, myIndexSetStrategy);
        }

        #region Set(myKeyValuePairs, myIndexSetStrategy)

        protected void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {
            _IVersionedIndexObject.Set(myKeyValuePairs, myIndexSetStrategy);
        }

        #endregion

        protected void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            _IVersionedIndexObject.Set(myDictionary, myIndexSetStrategy);
        }

        protected void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            _IVersionedIndexObject.Set(myMultiValueDictionary, myIndexSetStrategy);
        }

        protected Trinary ContainsKey(TKey myKey)
        {
            return _IVersionedIndexObject.ContainsKey(myKey);
        }

        protected Trinary ContainsValue(TValue myValue)
        {
            return _IVersionedIndexObject.ContainsValue(myValue);
        }

        protected Trinary Contains(TKey myKey, TValue myValue)
        {
            return _IVersionedIndexObject.Contains(myKey, myValue);
        }

        protected Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IVersionedIndexObject.Contains(myFunc);
        }

        protected HashSet<TValue> this[TKey myKey]
        {
            get
            {
                return _IVersionedIndexObject[myKey];
            }
            set
            {
                _IVersionedIndexObject[myKey] = value;
            }
        }

        protected bool TryGetValue(TKey myKey, out HashSet<TValue> myValue)
        {
            return _IVersionedIndexObject.TryGetValue(myKey, out myValue);
        }

        protected IEnumerable<TKey> Keys()
        {
            return _IVersionedIndexObject.Keys();
        }

        protected IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IVersionedIndexObject.Keys(myFunc);
        }

        protected ulong KeyCount()
        {
            return _IVersionedIndexObject.KeyCount();
        }

        protected ulong KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IVersionedIndexObject.KeyCount(myFunc);
        }

        protected IEnumerable<HashSet<TValue>> Values()
        {
            return _IVersionedIndexObject.Values();
        }

        protected IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IVersionedIndexObject.Values(myFunc);
        }

        protected ulong ValueCount()
        {
            return _IVersionedIndexObject.ValueCount();
        }

        protected ulong ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IVersionedIndexObject.ValueCount(myFunc);
        }

        protected IEnumerable<TValue> GetValues()
        {
            return _IVersionedIndexObject.GetValues();
        }

        protected IDictionary<TKey, HashSet<TValue>> GetIDictionary()
        {
            return _IVersionedIndexObject.GetIDictionary();
        }

        protected IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IVersionedIndexObject.GetIDictionary(myFunc);
        }

        protected IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
        {
            return _IVersionedIndexObject.GetEnumerator();
        }

        protected IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IVersionedIndexObject.GetEnumerator(myFunc);
        }

        protected bool Remove(TKey myKey)
        {
            return _IVersionedIndexObject.Remove(myKey);
        }

        protected bool Remove(TKey myKey, TValue myValue)
        {
            return _IVersionedIndexObject.Remove(myKey, myValue);
        }

        protected bool Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IVersionedIndexObject.Remove(myFunc);
        }

        protected void Clear()
        {
            _IVersionedIndexObject.Clear();
        }

        #endregion


        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            ((AFSObject)_IVersionedIndexObject).Serialize(ref mySerializationWriter);
        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            ((AFSObject)_IVersionedIndexObject).Deserialize(ref mySerializationReader);
        }

        public override AFSObject Clone()
        {
            return ((AFSObject)_IVersionedIndexObject).Clone();
        }


        #region IIndexObject<TKey,TValue> Members

        public IIndexObject<TKey, TValue> GetNewInstance()
        {
            return _IVersionedIndexObject.GetNewInstance();
        }

        #endregion

        #region IIndexInterface<TKey,TValue> Members

        public string IndexName
        {
            get { return _IVersionedIndexObject.IndexName; }
        }

        #endregion

        #region (protected) IVersionedIndexInterface<TKey,TValue> Members

        protected Trinary ContainsKey(TKey myKey, long myVersion)
        {
            return _IVersionedIndexObject.ContainsKey(myKey, myVersion);
        }

        protected Trinary ContainsValue(TValue myValue, long myVersion)
        {
            return _IVersionedIndexObject.ContainsValue(myValue, myVersion);
        }

        protected Trinary Contains(TKey myKey, TValue myValue, long myVersion)
        {
            return _IVersionedIndexObject.Contains(myKey, myValue, myVersion);
        }

        protected Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            return _IVersionedIndexObject.Contains(myFunc, myVersion);
        }

        protected HashSet<TValue> this[TKey myKey, long myVersion]
        {
            get { return _IVersionedIndexObject[myKey, myVersion]; }
        }

        protected bool TryGetValue(TKey myKey, out HashSet<TValue> myValue, long myVersion)
        {
            return _IVersionedIndexObject.TryGetValue(myKey, out myValue, myVersion);
        }

        protected IEnumerable<TKey> Keys(long myVersion)
        {
            return _IVersionedIndexObject.Keys(myVersion);
        }

        protected IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            return _IVersionedIndexObject.Keys(myFunc, myVersion);
        }

        protected ulong KeyCount(long myVersion)
        {
            return _IVersionedIndexObject.KeyCount(myVersion);
        }

        protected ulong KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            return _IVersionedIndexObject.KeyCount(myFunc, myVersion);
        }

        protected IEnumerable<HashSet<TValue>> Values(long myVersion)
        {
            return _IVersionedIndexObject.Values(myVersion);
        }

        protected IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            return _IVersionedIndexObject.Values(myFunc, myVersion);
        }

        protected ulong ValueCount(long myVersion)
        {
            return _IVersionedIndexObject.ValueCount(myVersion);
        }

        protected ulong ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            return _IVersionedIndexObject.ValueCount(myFunc, myVersion);
        }

        protected IDictionary<TKey, HashSet<TValue>> GetIDictionary(long myVersion)
        {
            return _IVersionedIndexObject.GetIDictionary(myVersion);
        }

        protected IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            return _IVersionedIndexObject.GetIDictionary(myFunc, myVersion);
        }

        protected IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(long myVersion)
        {
            return _IVersionedIndexObject.GetEnumerator(myVersion);
        }

        protected IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            return _IVersionedIndexObject.GetEnumerator(myFunc, myVersion);
        }

        protected ulong VersionCount(TKey myKey)
        {
            return _IVersionedIndexObject.VersionCount(myKey);
        }

        protected void ClearHistory(TKey myKey)
        {
            _IVersionedIndexObject.ClearHistory(myKey);
        }

        #endregion

    }

}
