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
    /// Root interface for all kinds of indices
    /// </summary>
    /// <typeparam name="TKey">The type of the comparable index key</typeparam>
    /// <typeparam name="TValue">The type of the index value</typeparam>
    public interface IIndex<TKey, TValue> : ISonesIndex
        where TKey : IComparable
    {
        #region General Properties

        /// <summary>
        /// Is this a persistent index?
        /// </summary>
        Boolean IsPersistent { get; }

        /// <summary>
        /// Returns the name of the index
        /// </summary>
        String Name { get; }

        #endregion

        #region Counts

        /// <summary>
        /// Returns the number of keys
        /// </summary>
        Int64 KeyCount();

        /// <summary>
        /// Returns the number of values
        /// </summary>
        /// <returns></returns>
        Int64 ValueCount();

        #endregion

        #region Keys

        /// <summary>
        /// Returns all keys of the index
        /// </summary>
        ISet<TKey> Keys();

        #endregion

        #region ContainsKey / -Value / -KeyValue

        /// <summary>
        /// Checks if a key exists in the index
        /// </summary>
        /// <param name="myKey">the key to be checked</param>
        /// <returns>true if the key exists, else false</returns>
        bool ContainsKey(TKey myKey);

        /// <summary>
        /// Checks if a value exists in the index
        /// </summary>
        /// <param name="myValue">the value to be checked</param>
        /// <returns>true if the value exists, else false</returns>
        bool ContainsValue(TValue myValue);

        /// <summary>
        /// Checks if a key and an associated value exists in the index
        /// </summary>
        /// <param name="myKey">the key</param>
        /// <param name="myValue">the associated value</param>
        /// <returns>true if the key and the associated value exist, else false</returns>
        bool Contains(TKey myKey, TValue myValue);

        #endregion

        #region Remove

        /// <summary>
        /// Removes a given key and its associated value from the index
        /// </summary>
        /// <param name="myKey">the key to be removed</param>
        /// <returns></returns>
        bool Remove(TKey myKey);

        /// <summary>
        /// Clears the index
        /// </summary>
        void ClearIndex();

        #endregion
    }
}