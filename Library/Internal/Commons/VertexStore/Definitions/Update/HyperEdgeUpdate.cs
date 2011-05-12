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

namespace sones.Library.Commons.VertexStore.Definitions.Update
{
    /// <summary>
    /// The class that defines updates on hyperedges of a vertex
    /// </summary>
    public sealed class HyperEdgeUpdate
    {
        #region data

        /// <summary>
        /// The hyperedges that are going to be updated
        /// </summary>
        public IDictionary<Int64, HyperEdgeUpdateDefinition> Updated;
          
        /// <summary>
        /// The hyperedge property-ids that are going to be deleted
        /// </summary>
        public IEnumerable<Int64> Deleted;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new hyper edge update
        /// </summary>
        /// <param name="myAdded">The hyperedges that are going to be added</param>
        /// <param name="myUpdated">The hyperedges that are going to be updated</param>
        /// <param name="myDeleted">The hyperedge property-ids that are going to be deleted</param>
        public HyperEdgeUpdate(
            IDictionary<Int64, HyperEdgeUpdateDefinition> myUpdated = null,
            IEnumerable<Int64> myDeleted = null)
        {
            Updated = myUpdated;
            Deleted = myDeleted;
        }

        #endregion
    }
}
