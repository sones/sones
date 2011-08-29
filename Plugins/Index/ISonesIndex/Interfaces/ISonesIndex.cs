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
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index.Helper;
using sones.Library.PropertyHyperGraph;

namespace sones.Plugins.Index
{
    #region ISonesIndexVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible ISonesIndex plugin versions. 
    /// Defines the min and max version for all ISonesIndex implementations which will be activated used this IIndex.
    /// </summary>
    public static class ISonesIndexVersionCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }

    #endregion

    /// <summary>
    /// This interface represents the default index
    /// usage in the sones GraphDB. It contains all 
    /// necessary index operations and is designed 
    /// for managing multiple value indices (there
    /// are 1 - n values assigned to a search key).
    /// </summary>
    public interface ISonesIndex
    {
        #region Name

        /// <summary>
        /// The name of the index
        /// </summary>
        String IndexName { get; }

        #endregion

        #region Counts

        /// <summary>
        /// Returns the number of stored keys
        /// </summary>
        /// <returns>Number of stored keys</returns>
        Int64 KeyCount();

        /// <summary>
        /// Returns the number of values
        /// </summary>
        /// <returns>Number of stored values</returns>
        Int64 ValueCount();

        #endregion

        #region Keys

        /// <summary>
        /// Returns all stored keys.
        /// </summary>
        /// <returns>All stored keys</returns>
        IEnumerable<IComparable> Keys();

        #endregion

        #region Init

        /// <summary>
        /// Inits the index and use property
        /// referenced by the given propertyID
        /// for indexing.
        /// </summary>
        /// <param name="myPropertyID">
        /// The propertyID of the index-key.
        /// </param>
        void Init(Int64 myPropertyID);

        #endregion

        #region GetKeyType

        /// <summary>
        /// Returns the type of the indexed key.
        /// </summary>
        /// <returns>
        /// The key type.
        /// </returns>
        Type GetKeyType();

        #endregion

        #region Add

        /// <summary>
        /// Add a vertexID to the index
        /// based on the specified vertex property.
        /// 
        /// The index will retrieve the property value
        /// of the indexed property and will store it.
        /// </summary>
        /// <param name="myVertex">
        /// My vertex.
        /// </param>
        /// <param name="myIndexAddStrategy">
        /// Defines what happens if a key already exists.
        /// </param>
        void Add(IVertex myVertex,
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE);

        /// <summary>
        /// Adds a collection of vertexIDs to the index
        /// based on the specified vertex property.
        /// </summary>
        /// <param name="myVertices">
        /// My vertices.
        /// </param>
        /// <param name="myIndexAddStrategy">
        /// Defines what happens if a key already exists.
        /// </param>
        void AddRange(IEnumerable<IVertex> myVertices, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE);

        /// <summary>
        /// Adds a Key-Value Pair to the index.
        /// </summary>
        /// <param name="myKey">Search key</param>
        /// <param name="myVertexID">VertexID</param>
        /// <param name="myIndexAddStrategy">Define what happens, if the key already exists.</param>
        void Add(IComparable myKey, Int64 myVertexID,
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE);

        /// <summary>
        /// Adds a collection of Key-Value Pairs to the index.
        /// </summary>
        /// <param name="myKeyValuePairs">Key-Value-Pairs</param>
        /// <param name="myIndexAddStrategy">Define what happens, if the key already exists.</param>
        void AddRange(IEnumerable<KeyValuePair<IComparable, Int64>> myKeyValuePairs,
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE);

        #endregion

        #region TryGetValue

        /// <summary>
        /// Selects all values of the index for the given key.
        /// 
        /// The key has to be of the same type as the indexed property.
        /// </summary>
        /// <returns>
        /// True if the index contains the given key.
        /// </returns>
        /// <param name="myKey">
        /// The key to search for.
        /// </param>
        /// <param name="myVertexIDs">
        /// Stores the vertexIDs if the key has been found.
        /// </param>
        bool TryGetValues(IComparable myKey, out IEnumerable<Int64> myVertexIDs);

        #endregion

        #region this

        /// <summary>
        /// Returns all values associated to the given key.
        /// 
        /// If the key doesn't exist a 
        /// <see cref="sones.Plugins.Index.ErrorHandling.IndexKeyNotFoundException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="myKey">The search key</param>
        /// <returns>Associated values</returns>
        IEnumerable<Int64> this[IComparable myKey] { get; }

        #endregion

        #region ContainsKey

        /// <summary>
        /// Checks if the given key exists in the index.
        /// 
        /// The type of the key has to be the type of the indexed property.
        /// </summary>
        /// <returns>
        /// True if the index contains the given key.
        /// </returns>
        /// <param name="myKey">
        /// The key to search for.
        /// </param>
        bool ContainsKey(IComparable myKey);

        #endregion

        #region Remove

        /// <summary>
        /// Remove the specified key from the index.
        /// 
        /// The type of the key has to be the type of the index property.
        /// </summary>
        /// <param name="myKey">
        /// The key to remove.
        /// </param>
        /// <returns>True, if the key has been removed.</returns>
        bool Remove(IComparable myKey);

        /// <summary>
        /// Removes the range of keys from the index.
        /// 
        /// Note, that there is no acknowledgement of deletion.
        /// </summary>
        /// <param name="myKeys">
        /// The keys to be removed.
        /// </param>
        void RemoveRange(IEnumerable<IComparable> myKeys);

        /// <summary>
        /// Removes a value associated with the given key 
        /// from the index.
        /// </summary>
        /// <param name="myKey">Search key</param>
        /// <param name="myValue">Value to be removed.</param>
        /// <returns>True, if the value has been removed.</returns>
        bool TryRemoveValue(IComparable myKey, Int64 myValue);

        /// <summary>
        /// Checks if the given vertex is indexed and if yes, removes
        /// the vertex id from the index.
        /// </summary>
        /// <param name="myVertex">Vertex which ID shall be removed</param>
        /// <returns>True, if the vertexID has been removed from the index.</returns>
        bool Remove(IVertex myVertex);

        #endregion

        #region Optimize

        /// <summary>
        /// Optimizes the index.
        /// </summary>
        void Optimize();

        #endregion

        #region Clear

        /// <summary>
        /// Removes all data from index
        /// </summary>
        void Clear();

        #endregion

        #region Additional Properties

        /// <summary>
        /// True, if <code>null</code> can be added as key.
        /// </summary>
        bool SupportsNullableKeys { get; }

        #endregion
    }
}
