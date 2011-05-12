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
    /// This struct represents the filesystem definition for a vertex
    /// </summary>
    public struct VertexAddDefinition
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
        /// The vertex id
        /// </summary>
        public readonly Int64 VertexID;

        /// <summary>
        /// The ID of the vertex type
        /// </summary>
        public readonly Int64 VertexTypeID;

        /// <summary>
        /// The binary properties
        /// </summary>
        public readonly IEnumerable<StreamAddDefinition> BinaryProperties;

        /// <summary>
        /// The edition of the vertex
        /// </summary>
        public readonly string Edition;

        /// <summary>
        /// The definition of the outgoing hyper edges
        /// </summary>
        public readonly IEnumerable<HyperEdgeAddDefinition> OutgoingHyperEdges;

        /// <summary>
        /// The definition of the outgoing hyper edges
        /// </summary>
        public readonly IEnumerable<SingleEdgeAddDefinition> OutgoingSingleEdges;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new vertex add definition
        /// </summary>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myEdition">The edition of the new vertex</param>
        /// <param name="myOutgoingHyperEdges">The outgoing hyper edge definitions</param>
        /// <param name="myOutgoingSingleEdges">The outgoing single edge definitions</param>
        /// <param name="myBinaryProperties">The binary properties of the new vertex</param>
        /// <param name="myComment">The comment on this graph element</param>
        /// <param name="myCreationDate">The creation date of this element</param>
        /// <param name="myModificationDate">The modification date of this element</param>
        /// <param name="myStructuredProperties">The structured properties of this element</param>
        /// <param name="myUnstructuredProperties">The unstructured properties of this element</param>
        public VertexAddDefinition(
            Int64 myVertexID,
            Int64 myVertexTypeID,
            String myEdition,
            IEnumerable<HyperEdgeAddDefinition> myOutgoingHyperEdges,
            IEnumerable<SingleEdgeAddDefinition> myOutgoingSingleEdges,
            IEnumerable<StreamAddDefinition> myBinaryProperties,
            String myComment,
            long myCreationDate,
            long myModificationDate,
            IDictionary<Int64, IComparable> myStructuredProperties,
            IDictionary<String, Object> myUnstructuredProperties)
        {
            Edition = !string.IsNullOrEmpty(myEdition) ? myEdition : ConstantsVertexStore.DefaultVertexEdition;

            VertexID = myVertexID;

            VertexTypeID = myVertexTypeID;

            OutgoingHyperEdges = myOutgoingHyperEdges;

            OutgoingSingleEdges = myOutgoingSingleEdges;

            BinaryProperties = myBinaryProperties;

            Comment = myComment;

            CreationDate = myCreationDate;

            ModificationDate = myModificationDate;

            StructuredProperties = myStructuredProperties;

            UnstructuredProperties = myUnstructuredProperties;
        }

        #endregion
    }
}