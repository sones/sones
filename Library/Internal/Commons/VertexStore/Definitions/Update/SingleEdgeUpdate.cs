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
    /// The class that defines updates on single edges
    /// </summary>
    public sealed class SingleEdgeUpdate
    {
        #region data

        /// <summary>
        /// The single edges that should be updated
        /// property id + single edge update definition
        /// </summary>
        public IDictionary<Int64, SingleEdgeUpdateDefinition> Updated;
        
        /// <summary>
        /// The single edge property-ids that should be deleted
        /// </summary>
        public IEnumerable<Int64> Deleted;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new single edge update
        /// </summary>
        /// <param name="myUpdated">The single edges that should be updated</param>
        /// <param name="myDeleted">The single edge property-ids that should be deleted</param>
        public SingleEdgeUpdate(
            IDictionary<Int64, SingleEdgeUpdateDefinition> myUpdated = null,
            IEnumerable<Int64> myDeleted = null)
        {
            Updated = myUpdated;
            Deleted = myDeleted;
        }

        #endregion
    }
}
