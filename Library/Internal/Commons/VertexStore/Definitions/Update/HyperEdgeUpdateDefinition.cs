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
    /// A update definition for the hyper edge
    /// </summary>
    public sealed class HyperEdgeUpdateDefinition : AGraphElementUpdateDefinition
    {
        #region data
        
        /// <summary>
        /// The single edges that should be deleted from the hyperedge
        /// </summary>
        public readonly IEnumerable<SingleEdgeDeleteDefinition> ToBeDeletedSingleEdges;
        
        /// <summary>
        /// The single edges that should be updated
        /// </summary>
        public readonly IEnumerable<SingleEdgeUpdateDefinition> ToBeUpdatedSingleEdges;

        /// <summary>
        /// The edge type id.
        /// </summary>
        public long EdgeTypeID { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new HyperEdgeUpdateDefinition
        /// </summary>
        /// <param name="myCommentUpdate">A comment for the graphelement</param>
        /// <param name="myUpdatedStructuredProperties">The structured properties</param>
        /// <param name="myUpdatedUnstructuredProperties">The unstructured properties</param>
        /// <param name="myToBeDeletedSingleEdges">The single edges that should be deleted from the hyperedge</param>
        /// <param name="myToBeUpdatedSingleEdges">The single edges that should be updated</param>
        public HyperEdgeUpdateDefinition(
            long myEdgeTypeID,
            String myCommentUpdate = null,
            StructuredPropertiesUpdate myUpdatedStructuredProperties = null,
            UnstructuredPropertiesUpdate myUpdatedUnstructuredProperties = null,
            IEnumerable<SingleEdgeDeleteDefinition> myToBeDeletedSingleEdges = null,
            IEnumerable<SingleEdgeUpdateDefinition> myToBeUpdatedSingleEdges = null)
            : base(myCommentUpdate, myUpdatedStructuredProperties, myUpdatedUnstructuredProperties)
        {
            EdgeTypeID = myEdgeTypeID;
            ToBeDeletedSingleEdges = myToBeDeletedSingleEdges;
            ToBeUpdatedSingleEdges = myToBeUpdatedSingleEdges;
        }

        #endregion
    }
}
