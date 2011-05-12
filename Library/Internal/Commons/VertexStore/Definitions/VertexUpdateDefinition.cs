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

using System.Collections.Generic;
using System;
using sones.Library.Commons.VertexStore.Definitions.Update;

namespace sones.Library.Commons.VertexStore.Definitions
{
    /// <summary>
    /// This class represents the filesystem update definition for a vertex
    /// </summary>
    public sealed class VertexUpdateDefinition : AGraphElementUpdateDefinition
    {
        #region data

        /// <summary>
        /// The binary properties
        /// </summary>
        public readonly BinaryPropertiesUpdate UpdatedBinaryProperties;

        /// <summary>
        /// The to be updated single edges
        /// </summary>
        public readonly SingleEdgeUpdate UpdatedSingleEdges;

        /// <summary>
        /// The to be updated hyper edges
        /// </summary>
        public readonly HyperEdgeUpdate UpdateHyperEdges;

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new vertex update definition
        /// </summary>
        /// <param name="myCommentUpdate">The comment update</param>
        /// <param name="myUpdatedStructuredProperties">The update for the structured properties</param>
        /// <param name="myUpdatedUnstructuredProperties">The update for the unstructured properties</param>
        /// <param name="myUpdatedBinaryProperties">The update for the binary properties</param>
        /// <param name="mySingleEdgeUpdate">The update for the single edges</param>
        /// <param name="myHyperEdgeUpdate">The update for the hyper edges</param>
        public VertexUpdateDefinition(
            String myCommentUpdate = null, 
            StructuredPropertiesUpdate myUpdatedStructuredProperties = null, 
            UnstructuredPropertiesUpdate myUpdatedUnstructuredProperties = null,
            BinaryPropertiesUpdate myUpdatedBinaryProperties = null,
            SingleEdgeUpdate mySingleEdgeUpdate = null,
            HyperEdgeUpdate myHyperEdgeUpdate = null)
            : base(myCommentUpdate, myUpdatedStructuredProperties, myUpdatedUnstructuredProperties)
        {
            UpdatedBinaryProperties = myUpdatedBinaryProperties;
            UpdatedSingleEdges = mySingleEdgeUpdate;
            UpdateHyperEdges = myHyperEdgeUpdate;
        } 
        
        #endregion
    }
}