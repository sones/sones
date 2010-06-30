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


/*
 * IDictionaryInterface
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.Lib.DataStructures.Dictionaries
{

    /// <summary>
    /// The interface of a DictionaryObject to store a mapping TKey => TValue.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>

    public interface IDictionaryInterface<TKey, TValue>
        where TKey : IComparable
    {

        #region Add - will fail if the key already exists

        /// <summary>
        /// Adds mykey and myValue, but fails if the key already exists.
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myValue">the value storing some information</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        UInt64 Add(TKey myKey, TValue myValue);

        /// <summary>
        /// Adds the given key-value-pair, but fails if the key already exists.
        /// </summary>
        /// <param name="myKeyValuePair">a key-value-pair</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        UInt64 Add(KeyValuePair<TKey, TValue> myKeyValuePair);

        /// <summary>
        /// Adds all given key-value-pairs in one atomic operation.
        /// Will fail if any key already exists.
        /// </summary>
        /// <param name="myKeyValuePairs">An IEnumerable of Key-Value-Pairs</param>
        /// <returns>1 for a success or 0 if any key already exists.</returns>
        UInt64 Add(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs);

        /// <summary>
        /// Adds all key-value-pairs of the given IDictionary in one atomic operation.
        /// Will fail if any key already exists.
        /// </summary>
        /// <param name="myIDictionary">An IDictionary</param>
        /// <returns>1 for a success or 0 if any key already exists.</returns>
        UInt64 Add(IDictionary<TKey, TValue> myIDictionary);

        #endregion

        #region Set

        UInt64 Set(TKey myKey, TValue myValue);

        UInt64 Set(KeyValuePair<TKey, TValue> myKeyValuePair);

        UInt64 Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy);

        UInt64 Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy);

        #endregion

        #region Replace - will fail if the key does not exist or the given value != actual value

        /// <summary>
        /// Replaces the value indexed by myKey with myNewValue as logn as the given myOldValue matched the actual value.
        /// Will fail if the key is not existent or the actual value is not equals myOldValue due to concurrency conflicts.
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myOldValue">the old value</param>
        /// <param name="myNewValue">the new value</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        UInt64 Replace(TKey myKey, TValue myOldValue, TValue myNewValue);

        #endregion


        #region Contains

        Trinary ContainsKey(TKey myKey);

        Trinary ContainsValue(TValue myValue);

        Trinary Contains(TKey myKey, TValue myValue);

        Trinary Contains(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc);

        #endregion

        #region Get/Keys/Values/Enumerator

        TValue                      this[TKey myKey] { get; set; }
        Boolean                     TryGetValue(TKey key, out TValue value);

        IEnumerable<TKey>           Keys();
        IEnumerable<TKey>           Keys(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc);
        UInt64                      KeyCount();        
        UInt64                      KeyCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc);

        IEnumerable<TValue>         Values();
        IEnumerable<TValue>         Values(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc);
        UInt64                      ValueCount();
        UInt64                      ValueCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc);

        IDictionary<TKey, TValue>   GetIDictionary();
        IDictionary<TKey, TValue>   GetIDictionary(params TKey[] myKeys);
        IDictionary<TKey, TValue>   GetIDictionary(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc);

        IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
        //System.Collections.IEnumerator GetEnumerator();
        IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc);

        #endregion

        #region Remove/Clear

        Boolean Remove(TKey myKey);

        Boolean Remove(TKey myKey, TValue myValue);

        Boolean Remove(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc);

        void Clear();

        #endregion

    }

}