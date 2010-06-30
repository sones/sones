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
 * IndexValueHistory
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.Lib.DataStructures.Indices
{

    /// <summary>
    /// This datastructure implements a versioned and serialized value
    /// to be used within index datastructures
    /// </summary>
    /// <typeparam name="TValue">The type of the stored value</typeparam>
    public class IndexValueHistory<TValue>
    {

        #region Properties

        public  UInt64            Timestamp   { get; set; }
        public  HashSet<TValue>   AddSet      { get; set; }
        public  HashSet<TValue>   RemSet      { get; set; }

        #endregion


        #region Constructors

        #region IndexValueHistory()

        /// <summary>
        /// Creates a new IndexValueHistory using default values
        /// </summary>
        public IndexValueHistory()
        {
            Timestamp           = default(Int64);
            AddSet              = new HashSet<TValue>();
            RemSet              = new HashSet<TValue>();
        }

        #endregion

        #region IndexValueHistory(myAddSet, myRemSet)

        /// <summary>
        /// Creates a new IndexValueHistory, setting the internal value to the content of myValue
        /// </summary>
        public IndexValueHistory(IEnumerable<TValue> myAddSet, IEnumerable<TValue> myRemSet)
            : this(TimestampNonce.Ticks, myAddSet, myRemSet)
        {
        }

        #endregion

        #region IndexValueHistory(myTimestamp, myAddSet, myRemSet)

        /// <summary>
        /// Creates a new IndexValueHistory, setting the internal value to the content of myValue
        /// </summary>
        public IndexValueHistory(UInt64 myTimestamp, IEnumerable<TValue> myAddSet, IEnumerable<TValue> myRemSet)
        {

            Timestamp = myTimestamp;

            if (myAddSet == null)
                AddSet = new HashSet<TValue>();
            else
                AddSet = new HashSet<TValue>(myAddSet);

            if (myRemSet == null)
                RemSet = new HashSet<TValue>();
            else
                RemSet = new HashSet<TValue>(myRemSet);

        }

        #endregion

        #endregion


        #region GetHashCode()

        public override int GetHashCode()
        {
            return Timestamp.GetHashCode() ^ AddSet.GetHashCode() ^ RemSet.GetHashCode();
        }

        #endregion

    }

}
