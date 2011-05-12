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

namespace sones.Library.Commons.VertexStore.Definitions
{
    /// <summary>
    /// This struct represents the filesystem definition for an edge
    /// </summary>
    public struct HyperEdgeAddDefinition
    {
        #region data

        /// <summary>
        /// A comment for the vertex
        /// </summary>
        public readonly string Comment;

        /// <summary>
        /// The creation date of the vertex
        /// </summary>
        public readonly long CreationDate;

        /// <summary>
        /// The modification date of the vertex
        /// </summary>
        public readonly long ModificationDate;

        /// <summary>
        /// The structured properties
        /// </summary>
        public readonly IDictionary<Int64, IComparable> StructuredProperties;

        /// <summary>
        /// The unstructured properties
        /// </summary>
        public readonly IDictionary<String, Object> UnstructuredProperties;

        /// <summary>
        /// The property id of the edge
        /// </summary>
        public readonly Int64 PropertyID;

        /// <summary>
        /// The edge type id of this edge
        /// </summary>
        public readonly Int64 EdgeTypeID;

        /// <summary>
        /// The single edges that are contained in this hyperedge
        /// </summary>
        public readonly IEnumerable<SingleEdgeAddDefinition> ContainedSingleEdges;

        /// <summary>
        /// The source vertex for this hyper edge
        /// </summary>
        public readonly VertexInformation SourceVertex;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new hyper edge definition
        /// </summary>
        /// <param name="myPropertyID">The property id of the edge</param>
        /// <param name="myEdgeTypeID">The id of this edge type</param>
        /// <param name="mySourceVertex">The source vertex of this hyper edge</param>
        /// <param name="myContainedSingleEdges">The single edges that are contained within this hyper edge</param>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        public HyperEdgeAddDefinition(
            Int64 myPropertyID,
            Int64 myEdgeTypeID,
            VertexInformation mySourceVertex,
            IEnumerable<SingleEdgeAddDefinition> myContainedSingleEdges,
            String myComment,
            long myCreationDate,
            long myModificationDate,
            IDictionary<Int64, IComparable> myStructuredProperties,
            IDictionary<String, Object> myUnstructuredProperties)
        {
            PropertyID = myPropertyID;
            EdgeTypeID = myEdgeTypeID;
            SourceVertex = mySourceVertex;
            ContainedSingleEdges = myContainedSingleEdges;
            Comment = myComment;
            CreationDate = myCreationDate;
            ModificationDate = myModificationDate;
            StructuredProperties = myStructuredProperties;
            UnstructuredProperties = myUnstructuredProperties;
        }

        #endregion
    }
}