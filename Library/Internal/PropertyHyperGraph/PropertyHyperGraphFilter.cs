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
using System.IO;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// Static filter class
    /// </summary>
    public static class PropertyHyperGraphFilter
    {
        #region IVertex

        /// <summary>
        /// Filters the incoming edges of a vertex
        /// </summary>
        /// <param name="myIncomingVertexType">The id of the incoming vertex type</param>
        /// <param name="myIncomingEdgePropertyID">The id of the incoming edge property</param>
        /// <param name="myIncomingVertices">The incoming vertices</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool IncomingVerticesFilter(Int64 myIncomingVertexType, Int64 myIncomingEdgePropertyID, IEnumerable<IVertex> myIncomingVertices);

        /// <summary>
        /// Filters all outgoing edges of a vertex
        /// </summary>
        /// <param name="myEdgePropertyID">The edge property id</param>
        /// <param name="myOutgoingEdge">The outgoing edge</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool OutgoingEdgeFilter(Int64 myEdgePropertyID, IEdge myOutgoingEdge);
        
        /// <summary>
        /// Filters all outgoing hyper edges of a vertex
        /// </summary>
        /// <param name="myEdgePropertyID">The edge property id</param>
        /// <param name="myOutgoingEdge">The outgoing hyper edge</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool OutgoingHyperEdgeFilter(Int64 myEdgePropertyID, IHyperEdge myOutgoingEdge);

        /// <summary>
        /// Filters all outgoing single edges
        /// </summary>
        /// <param name="myEdgePropertyID">The edge property id</param>
        /// <param name="myOutgoingEdge">The outgoing single edge</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool OutgoingSingleEdgeFilter(Int64 myEdgePropertyID, ISingleEdge myOutgoingEdge);

        /// <summary>
        /// Filters all binary properties of a vertex
        /// </summary>
        /// <param name="myBinaryPropertyID">The binary property id</param>
        /// <param name="myBinaryStream">The stream</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool BinaryPropertyFilter(Int64 myBinaryPropertyID, Stream myBinaryStream);

        #endregion

        #region IGraphElement

        /// <summary>
        /// Filters a graph element property
        /// </summary>
        /// <param name="myStructuredPropertyID">The id of the property</param>
        /// <param name="myProperty">The property</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool GraphElementStructuredPropertyFilter(Int64 myStructuredPropertyID, IComparable myProperty);

        /// <summary>
        /// Filters an unstructured graph element property
        /// </summary>
        /// <param name="myUnstructuredPropertyName">The name of the unstructured property</param>
        /// <param name="myProperty">The property</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool GraphElementUnStructuredPropertyFilter(String myUnstructuredPropertyName, Object myProperty);

        #endregion

        #region IHyperEdge

        /// <summary>
        /// A filter for a single edge of a hyperedge
        /// </summary>
        /// <param name="mySingleEdge">The single edge</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool SingleEdgeFilter(ISingleEdge mySingleEdge);

        #endregion

        #region IEdge

        /// <summary>
        /// Filter a target vertex
        /// </summary>
        /// <param name="myVertex">The vertex</param>
        /// <returns>False means: I do not want that thing, otherwise true</returns>
        public delegate bool TargetVertexFilter(IVertex myVertex);

        #endregion
    }
}
