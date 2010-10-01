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

/*
 * IIndexInterface
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Collections;

#endregion

namespace sones.Lib.DataStructures.Indices
{

    /// <summary>
    /// The interface for all indices
    /// </summary>

    public interface IIndexInterface<TKey, TValue> : IEnumerable<KeyValuePair<TKey, HashSet<TValue>>>
        where TKey : IComparable
    {

        #region IndexName

        String IndexName { get; }

        #endregion

        #region Add

        void Add(TKey myKey, TValue myValue);

        void Add(TKey myKey, IEnumerable<TValue> myValues);

        void Add(KeyValuePair<TKey, TValue> myKeyValuePair);

        void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair);

        void Add(Dictionary<TKey, TValue> myDictionary);

        void Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary);

        #endregion

        #region Set

        void Set(TKey myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy);

        void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy);

        void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy);

        void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy);

        void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy);

        void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy);

        void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy);

        #endregion

        #region Contains

        Trinary ContainsKey(TKey myKey);

        Trinary ContainsValue(TValue myValue);

        Trinary Contains(TKey myKey, TValue myValue);

        Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc);

        #endregion

        #region Get/Keys/Values/Enumerator

        HashSet<TValue>                 this[TKey key] { get; set; }
        Boolean                         TryGetValue(TKey key, out HashSet<TValue> value);

        IEnumerable<TKey>               Keys();
        IEnumerable<TKey>               Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc);
        UInt64                          KeyCount();
        UInt64                          KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc);

        IEnumerable<HashSet<TValue>>    Values();
        IEnumerable<HashSet<TValue>>    Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc);
        IEnumerable<TValue>             GetValues();
        UInt64                          ValueCount();
        UInt64                          ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc);

        IDictionary<TKey, HashSet<TValue>> GetIDictionary();
        IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc);

        IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc);

        #endregion

        #region Remove/Clear

        Boolean Remove(TKey myKey);

        Boolean Remove(TKey myKey, TValue myValue);

        Boolean Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc);

        void Clear();

        #endregion

        #region Range Queries

        IEnumerable<TValue> GreaterThan(TKey myKey, bool myOrEqual = true);

        IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, bool myOrEqual = true);

        IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true);

        IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, bool myOrEqual = true);

        IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true);

        IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, bool myOrEqualFromKey = true, bool myOrEqualToKey = true);

        #endregion


    }

}