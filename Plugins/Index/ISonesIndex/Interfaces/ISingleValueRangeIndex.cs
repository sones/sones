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

using System;
using System.Collections.Generic;

namespace sones.Plugins.Index.Interfaces
{
    /// <summary>
    /// Interface defines indices which have a 1:1 key-value relationship
    /// and support range queries on index keys.
    /// </summary>
    /// <typeparam name="TKey">Type of the index-key</typeparam>
    /// <typeparam name="TValue">Type of the values</typeparam>
    public interface ISingleValueRangeIndex<TKey, TValue> :
        ISingleValueIndex<TKey, TValue>,
        IRangeIndex<TKey, TValue>
        where TKey : IComparable
    {
        #region Range

        /// <summary>
        /// Returns all values from keys which are greater than the given key
        /// </summary>
        /// <param name="myKey">the lower bound of the range</param>
        /// <param name="myOrEqual">true if the key shall be included in the range</param>
        /// <returns>values from all keys greater than given key</returns>
        ISet<TValue> GreaterThan(TKey myKey, bool myOrEqual = true);

        /// <summary>
        /// Returns all values from keys which are lower than the given key
        /// </summary>
        /// <param name="myKey">the upper bound of the range</param>
        /// <param name="myOrEqual">true if the key shall be included in the range</param>
        /// <returns>values from all keys lower than given key</returns>
        ISet<TValue> LowerThan(TKey myKey, bool myOrEqual = true);

        /// <summary>
        /// Returns all values from keys in a given range
        /// </summary>
        /// <param name="myFromKey">the lower bound of the range</param>
        /// <param name="myToKey">the upper bound of the range</param>
        /// <param name="myOrEqualFromKey">true if the lower bound shall be included in the range</param>
        /// <param name="myOrEqualToKey">true if the upper bound shall be included in the range</param>
        /// <returns>values from all keys in the given range</returns>
        ISet<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true,
                                    bool myOrEqualToKey = true);

        #endregion
    }
}