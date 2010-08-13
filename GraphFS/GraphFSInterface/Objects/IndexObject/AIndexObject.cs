/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.DataStructures;
using sones.Lib.NewFastSerializer;
using sones.GraphFS.Session;
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.WeakReference;

namespace sones.GraphFS.Objects
{

    public abstract class AIndexObject<TKey, TValue> : AFSObject
        where TKey : IComparable
    {

        #region Data

        protected IIndexObject<TKey, TValue> _IIndexObject;

        #endregion

        #region Constructors

        public AIndexObject()
        {
            _IIndexObject = new HashIndexObject<TKey, TValue>();
        }

        public AIndexObject(IIndexObject<TKey, TValue> myIIndexObject)
        {
            _IIndexObject = myIIndexObject;
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
                return ((AFSObject)_IIndexObject).IGraphFSReference;
            }

            set
            {
                _IGraphFSReference = value;
                ((AFSObject)_IIndexObject).IGraphFSReference = value;
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
                return ((AFSObject)_IIndexObject).IGraphFSSessionReference;
            }

            set
            {
                _IGraphFSSessionReference = value;
                ((AFSObject)_IIndexObject).IGraphFSSessionReference = value;
            }

        }

        #endregion


        #region (protected) IIndexObject<TKey,TValue> Members

        protected void Add(TKey myKey, TValue myValue)
        {
            _IIndexObject.Add(myKey, myValue);
        }

        protected void Add(TKey myKey, IEnumerable<TValue> myValues)
        {
            _IIndexObject.Add(myKey, myValues);
        }

        protected void Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            _IIndexObject.Add(myKeyValuePair);
        }

        protected void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
        {
            _IIndexObject.Add(myKeyValuesPair);
        }

        protected void Add(Dictionary<TKey, TValue> myDictionary)
        {
            _IIndexObject.Add(myDictionary);
        }

        protected void Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary)
        {
            _IIndexObject.Add(myDictionary);
        }



        protected void Set(TKey myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
        {
            _IIndexObject.Set(myKey, myValue, myIndexSetStrategy);
        }

        protected void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
        {
            _IIndexObject.Set(myKey, myValues, myIndexSetStrategy);
        }

        protected void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
        {
            _IIndexObject.Set(myKeyValuePair, myIndexSetStrategy);
        }

        protected void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
        {
            _IIndexObject.Set(myKeyValuesPair, myIndexSetStrategy);
        }

        #region Set(myKeyValuePairs, myIndexSetStrategy)

        protected void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {
            _IIndexObject.Set(myKeyValuePairs, myIndexSetStrategy);
        }

        #endregion

        protected void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            _IIndexObject.Set(myDictionary, myIndexSetStrategy);
        }

        protected void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            _IIndexObject.Set(myMultiValueDictionary, myIndexSetStrategy);
        }

        protected Trinary ContainsKey(TKey myKey)
        {
            return _IIndexObject.ContainsKey(myKey);
        }

        protected Trinary ContainsValue(TValue myValue)
        {
            return _IIndexObject.ContainsValue(myValue);
        }

        protected Trinary Contains(TKey myKey, TValue myValue)
        {
            return _IIndexObject.Contains(myKey, myValue);
        }

        protected Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IIndexObject.Contains(myFunc);
        }

        protected HashSet<TValue> this[TKey myKey]
        {
            get
            {
                return _IIndexObject[myKey];
            }
            set
            {
                _IIndexObject[myKey] = value;
            }
        }

        protected bool TryGetValue(TKey myKey, out HashSet<TValue> myValue)
        {
            return _IIndexObject.TryGetValue(myKey, out myValue);
        }

        protected IEnumerable<TKey> Keys()
        {
            return _IIndexObject.Keys();
        }

        protected IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IIndexObject.Keys(myFunc);
        }

        protected ulong KeyCount()
        {
            return _IIndexObject.KeyCount();
        }

        protected ulong KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IIndexObject.KeyCount(myFunc);
        }

        protected IEnumerable<HashSet<TValue>> Values()
        {
            return _IIndexObject.Values();
        }

        protected IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IIndexObject.Values(myFunc);
        }

        protected ulong ValueCount()
        {
            return _IIndexObject.ValueCount();
        }

        protected ulong ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IIndexObject.ValueCount(myFunc);
        }

        protected IEnumerable<TValue> GetValues()
        {
            return _IIndexObject.GetValues();
        }

        protected IDictionary<TKey, HashSet<TValue>> GetIDictionary()
        {
            return _IIndexObject.GetIDictionary();
        }

        protected IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IIndexObject.GetIDictionary(myFunc);
        }

        protected IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
        {
            return _IIndexObject.GetEnumerator();
        }

        protected IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IIndexObject.GetEnumerator(myFunc);
        }

        protected bool Remove(TKey myKey)
        {
            return _IIndexObject.Remove(myKey);
        }

        protected bool Remove(TKey myKey, TValue myValue)
        {
            return _IIndexObject.Remove(myKey, myValue);
        }

        protected bool Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _IIndexObject.Remove(myFunc);
        }

        protected void Clear()
        {
            _IIndexObject.Clear();
        }

        #endregion


        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            ((AFSObject)_IIndexObject).Serialize(ref mySerializationWriter);
        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            ((AFSObject)_IIndexObject).Deserialize(ref mySerializationReader);
        }

        public override AFSObject Clone()
        {
            return ((AFSObject)_IIndexObject).Clone();
        }

    }

}
