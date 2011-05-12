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
    /// The update definition for single edges
    /// </summary>
    public sealed class SingleEdgeUpdateDefinition : AGraphElementUpdateDefinition
    {
        #region data

        /// <summary>
        /// The source for this single edge.
        /// </summary>
        public VertexInformation SourceVertex { get; private set; }

        /// <summary>
        /// The target for this edge.
        /// </summary>
        public VertexInformation TargetVertex { get; private set; }

        /// <summary>
        /// The edge type id.
        /// </summary>
        public  long EdgeTypeID { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new single edge update definition
        /// </summary>
        /// <param name="myCommentUpdate">A comment for the graphelemen</param>
        /// <param name="myUpdatedStructuredProperties">The structured properties</param>
        /// <param name="myUpdatedUnstructuredProperties">The unstructured properties</param>
        /// <param name="myUpdatedVector">Defines the single edge that should be updated</param>
        public SingleEdgeUpdateDefinition(
            VertexInformation mySourceVertex,
            VertexInformation myTargetVertex,
            long myEdgeTypeID,
            String myCommentUpdate = null,
            StructuredPropertiesUpdate myUpdatedStructuredProperties = null,
            UnstructuredPropertiesUpdate myUpdatedUnstructuredProperties = null)
            : base(myCommentUpdate, myUpdatedStructuredProperties, myUpdatedUnstructuredProperties)
        {
            SourceVertex = mySourceVertex;
            TargetVertex = myTargetVertex;
            EdgeTypeID = myEdgeTypeID;
        }

        #endregion
    
    }
}
