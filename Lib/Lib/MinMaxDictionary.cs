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


/* PandoraFS MinMaxDictionary
 * Achim Friedland, 2008
 * 
 * This is a generic dictionary which knows the mix and getLatestRevisionTime
 * values of its UInt64 keys.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace sones.Lib
{

    /// <summary>
    /// This is a generic dictionary which knows the mix and getLatestRevisionTime
    /// values of its UInt64 keys.
    /// </summary>

    

    public class MinMaxDictionary<T>
    {

        #region Data

        protected Dictionary<UInt64, T>  internalDictionary  = null;
        protected UInt64                 minKey              = UInt64.MaxValue;
        protected UInt64                 maxKey              = UInt64.MinValue;

        #endregion

        #region Constructor

        public MinMaxDictionary()
        {
            internalDictionary  = new Dictionary<UInt64, T>();
        }

        #endregion


        #region Internal Methods

        public int Count
        {
            get { return internalDictionary.Count; }
        }


        public Dictionary<UInt64, T>.KeyCollection Keys
        {
            get { return internalDictionary.Keys; }
        }


        public Dictionary<UInt64, T>.ValueCollection Values
        {
            get { return internalDictionary.Values; }
        }


        public T this[UInt64 key]
        {

            get { return internalDictionary[key]; }

            set
            {
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
                internalDictionary[key] = value;
            }

        }


        public void Add(UInt64 key, T value)
        {
            if (key < minKey) minKey = key;
            if (key > maxKey) maxKey = key;
            internalDictionary[key] = value;
        }


        public bool ContainsKey(UInt64 key)
        {
            return internalDictionary.ContainsKey(key);
        }


        public bool ContainsValue(T value)
        {
            return internalDictionary.ContainsValue(value);
        }


        public override bool Equals(object obj)
        {
            return internalDictionary.Equals(obj);
        }


        public IEnumerator GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }


        public override int GetHashCode()
        {
            return internalDictionary.GetHashCode();
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            internalDictionary.GetObjectData(info, context);
        }


        public void OnDeserialization(object sender)
        {
            internalDictionary.OnDeserialization(sender);
        }


        public override string ToString()
        {
            return internalDictionary.ToString();
        }


        public bool TryGetValue(UInt64 key, out T value)
        {
            return internalDictionary.TryGetValue(key, out value);
        }


        public void Remove(UInt64 key)
        {
            internalDictionary.Remove(key);
        }


        public void Clear()
        {
            internalDictionary.Clear();
        }

        #endregion


        #region Additional Methods

        public UInt64 MinKey()
        {
            return minKey;
        }


        public UInt64 MaxKey()
        {
            return maxKey;
        }


        public T MinValue()
        {
            return internalDictionary[minKey];
        }


        public T MaxValue()
        {
            return internalDictionary[maxKey];
        }

        #endregion


    }

}
