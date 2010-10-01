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
 * IGraphFSDictionary
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

#endregion

namespace sones.Lib.DataStructures
{

    /// <summary>
    /// A better IDictionary&lt;TKey, TValue&gt; interface
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>

    public interface IGraphFSDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {

        /// <summary>
        /// Adds an element with the provided key and value to the IGraphDictionary&lt;TKey, TValue&gt;.
        /// </summary>
        /// <param name="myKey">The object to use as the key of the element to add.</param>
        /// <param name="myValue">The object to use as the value of the element to add.</param>
        /// <returns>false if myKey is null, already exists or IGraphDictionary&lt;TKey, TValue&gt; is read-only</returns>
        Boolean Add(TKey myKey, TValue myValue);

        /// <summary>
        /// Adds an element with the provided key and value to the IGraphDictionary&lt;TKey, TValue&gt;.
        /// </summary>
        /// <param name="myKeyValuePair">The KeyValuePair&lt;TKey, TValue&gt; to add.</param>
        /// <returns>false if KeyValuePair.Key is null, already exists or IGraphDictionary&lt;TKey, TValue&gt; is read-only</returns>
        Boolean Add(KeyValuePair<TKey, TValue> myKeyValuePair);


        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="myKey">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        TValue this[TKey myKey] { get; set; }


        /// <summary>
        /// Determines whether the IGraphDictionary&lt;TKey, TValue&gt; contains an element with the specified key.
        /// </summary>
        /// <param name="myKey">The key to locate in the IGraphDictionary&lt;TKey, TValue&gt;.</param>
        /// <returns>true if the IGraphDictionary&lt;TKey, TValue&gt; contains an element with the key; otherwise, and if the key is null false.</returns>
        Boolean ContainsKey(TKey myKey);

        /// <summary>
        /// Determines whether the IGraphDictionary&lt;TKey, TValue&gt; contains the specified element.
        /// </summary>
        /// <param name="myKeyValuePair">The KeyValuePair to locate in the IGraphDictionary&lt;TKey, TValue&gt;.</param>
        /// <returns>true if the IGraphDictionary&lt;TKey, TValue&gt; contains the specified element; otherwise, and if the KeyValuePair.Key is null false.</returns>
        Boolean Contains(KeyValuePair<TKey, TValue> myKeyValuePair);


        /// <summary>
        /// Gets an IEnumerable&lt;TKey&gt; containing the keys of the IGraphDictionary&lt;TKey, TValue&gt;.
        /// </summary>
        IEnumerable<TKey> Keys { get; }

        /// <summary>
        /// Gets an IEnumerable&lt;TValue&gt; containing the keys of the IGraphDictionary&lt;TKey, TValue&gt;.
        /// </summary>
        IEnumerable<TValue> Values { get; }


        /// <summary>
        /// Gets the number of elements contained in the IGraphDictionary&lt;TKey, TValue&gt;.
        /// </summary>
        UInt64 Count { get; }


        /// <summary>
        /// Removes the element with the specified key from the IGraphDictionary&lt;TKey, TValue&gt;.
        /// </summary>
        /// <param name="myKey">The key of the element to remove.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the IGraphDictionary&lt;TKey, TValue&gt;.</returns>
        Boolean Remove(TKey myKey);

        /// <summary>
        /// Removes the element with the specified key and value from the IGraphDictionary&lt;TKey, TValue&gt;.
        /// </summary>
        /// <param name="myKeyValuePair">The KeyValuePair&lt;TKey, TValue&gt; to add.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if KeyValuePair.Key was not found in the IGraphDictionary&lt;TKey, TValue&gt;.</returns>
        Boolean Remove(KeyValuePair<TKey, TValue> myKeyValuePair);


        /// <summary>
        /// Removes all items from the IGraphDictionary&lt;TKey, TValue&gt;.
        /// </summary>
        void Clear();

    }
}
