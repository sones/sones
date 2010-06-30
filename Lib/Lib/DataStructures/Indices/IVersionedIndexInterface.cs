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
 * IVersionedIndexInterface
 * Achim Friedland, 2009 - 2010
 * 
 * <developer>Martin Junghanns</developer>
 */

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace sones.Lib.DataStructures.Indices
{

    /// <summary>
    /// The additional interface for all versioned IndexObjects
    /// </summary>

    public interface IVersionedIndexInterface<TKey, TValue> : IIndexInterface<TKey, TValue>
        where TKey : IComparable
    {

        #region Contains

        Trinary ContainsKey(TKey myKey, Int64 myVersion);

        Trinary ContainsValue(TValue myValue, Int64 myVersion);

        Trinary Contains(TKey myKey, TValue myValue, Int64 myVersion);

        Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion);

        #endregion

        #region Get/Keys/Values/Enumerator

        HashSet<TValue>                 this[TKey myKey, Int64 myVersion] { get; }
        Boolean                         TryGetValue(TKey key, out HashSet<TValue> value, Int64 myVersion);

        IEnumerable<TKey>               Keys(Int64 myVersion);
        IEnumerable<TKey>               Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion);
        UInt64                          KeyCount(Int64 myVersion);
        UInt64                          KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion);

        IEnumerable<HashSet<TValue>>    Values(Int64 myVersion);
        IEnumerable<HashSet<TValue>>    Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion);
        UInt64                          ValueCount(Int64 myVersion);
        UInt64                          ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion);

        IDictionary<TKey, HashSet<TValue>> GetIDictionary(Int64 myVersion);
        IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion);

        IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Int64 myVersion);
        IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion);

        #endregion

        #region Additional methods

        UInt64 VersionCount(TKey myKey);

        void ClearHistory(TKey myKey);

        #endregion

        #region Range Queries

        IEnumerable<TValue> GreaterThan(TKey myKey, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> LowerThan(TKey myKey, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion, bool myOrEqual = true);

        IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Int64 myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true);

        IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true);

        #endregion

    }

}
