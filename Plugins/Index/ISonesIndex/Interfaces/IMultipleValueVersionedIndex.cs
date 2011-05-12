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
using sones.Plugins.Index.Helper;

namespace sones.Plugins.Index.Interfaces
{
    /// <summary>
    /// Interface defines indices which have a 1:n key-value relationship
    /// and can handle different versions of the index.
    /// </summary>
    /// <typeparam name="TKey">Type of the index-key</typeparam>
    /// <typeparam name="TValue">Type of the index-values</typeparam>
    /// <typeparam name="TVersion">The type of comparable versions</typeparam>
    public interface IMultipleValueVersionedIndex<TKey, TValue, TVersion> :
        IMultipleValueIndex<TKey, TValue>,
        IVersionedIndex<TKey, TValue, TVersion>
        where TKey : IComparable
        where TVersion : IComparable
    {
        #region Add

        /// <summary>
        /// Gets / Sets the associated values of the key
        /// </summary>
        /// <param name="myKey">the index key</param>
        /// <param name="myVersion">the version of the index</param>
        /// <returns>set returns the values (if it exists)</returns>
        ISet<TValue> this[TKey myKey, TVersion myVersion] { get; set; }

        /// <summary>
        /// Adds a key and an associated value-set to the index
        /// </summary>
        /// <param name="myKey">the key to be inserted</param>
        /// <param name="myValue">the associated values</param>
        /// <param name="myVersion">the version of the index</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(TKey myKey, ISet<TValue> myValue, TVersion myVersion,
                 IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        /// <summary>
        /// Adds a KeyValuePair of key-values to the index
        /// </summary>
        /// <param name="myKeyValuePair">KeyValuePair</param>
        /// <param name="myVersion">the version of the index</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(KeyValuePair<TKey, ISet<TValue>> myKeyValuePair, TVersion myVersion,
                 IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        /// <summary>
        /// Adds a dictionary containing keys and the associated values to the graph
        /// </summary>
        /// <param name="myDictionary">a dictionary containing keys and the associated values</param>
        /// <param name="myVersion">the version of the index</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(Dictionary<TKey, ISet<TValue>> myDictionary, TVersion myVersion,
                 IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        #endregion

        #region Values

        /// <summary>
        /// Returns all value-sets of a given version
        /// </summary>
        /// <param name="myVersion">the version</param>
        /// <returns>all values of the given version</returns>
        IEnumerable<ISet<TValue>> Values(TVersion myVersion);

        #endregion
    }
}